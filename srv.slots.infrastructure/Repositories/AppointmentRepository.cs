using Dapper;
using srv.slots.application.Common.Exceptions;
using srv.slots.application.DTOs.Appointment;
using srv.slots.application.Interfaces.Repositories;
using srv.slots.infrastructure.Persistence;

namespace srv.slots.infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly IDbConnectionFactory _factory;
    public AppointmentRepository(IDbConnectionFactory factory) => _factory = factory;

    // ── Provider ──────────────────────────────────────────────────────────────

    public async Task<List<AppointmentDto>> GetByProviderDateAsync(uint providerId, DateTime date)
    {
        const string sql = @"
            SELECT a.id                  AS Id,
                   a.booking_ref         AS BookingRef,
                   a.appointment_type    AS AppointmentType,
                   a.appointment_date    AS AppointmentDate,
                   a.appointment_time    AS AppointmentTime,
                   a.token_number        AS TokenNumber,
                   a.status              AS Status,
                   a.patient_name        AS PatientName,
                   a.patient_age         AS PatientAge,
                   a.visit_reason        AS VisitReason,
                   a.provider_note       AS ProviderNote,
                   a.rejection_reason    AS RejectionReason,
                   a.cancellation_reason AS CancellationReason,
                   a.cancelled_by        AS CancelledBy,
                   a.created_at          AS CreatedAt,
                   a.customer_id         AS CustomerId,
                   c.full_name           AS CustomerName,
                   c.mobile              AS CustomerMobile,
                   a.service_id          AS ServiceId,
                   ps.name               AS ServiceName,
                   sl.start_time         AS StartTime,
                   sl.end_time           AS EndTime
            FROM   appointments          a
            JOIN   customers             c  ON c.id  = a.customer_id
            LEFT JOIN provider_services  ps ON ps.id = a.service_id
            LEFT JOIN provider_slots     sl ON sl.id = a.slot_id
            WHERE  a.provider_id      = @PId
              AND  a.appointment_date = @Date
            ORDER BY a.token_number, a.appointment_time;";

        using var conn = _factory.CreateConnection();
        return (await conn.QueryAsync<AppointmentDto>(sql,
                    new { PId = providerId, Date = date.Date })).ToList();
    }

    public async Task<bool> UpdateStatusAsync(uint id, uint providerId, UpdateAppointmentStatusDto dto)
    {
        const string sql = @"
            UPDATE appointments
            SET    status              = @Status,
                   rejection_reason   = CASE WHEN @Status = 'Rejected'  THEN @Rejection   ELSE rejection_reason   END,
                   cancellation_reason= CASE WHEN @Status = 'Cancelled' THEN @Cancellation ELSE cancellation_reason END,
                   cancelled_by       = CASE WHEN @Status = 'Cancelled' THEN 'Provider'   ELSE cancelled_by        END,
                   provider_note      = COALESCE(@Note, provider_note),
                   confirmed_at       = CASE WHEN @Status = 'Confirmed' THEN NOW() ELSE confirmed_at  END,
                   completed_at       = CASE WHEN @Status = 'Completed' THEN NOW() ELSE completed_at  END,
                   updated_at         = NOW()
            WHERE  id = @Id AND provider_id = @PId;";

        using var conn = _factory.CreateConnection();
        return await conn.ExecuteAsync(sql, new
        {
            Id           = id,
            PId          = providerId,
            Status       = dto.Status,
            Rejection    = dto.RejectionReason,
            Cancellation = dto.CancellationReason,
            Note         = dto.ProviderNote
        }) > 0;
    }

    public async Task<AppointmentStatsDto> GetTodayStatsAsync(uint providerId)
    {
        const string sql = @"
            SELECT COUNT(*)                          AS TotalToday,
                   SUM(status = 'Pending')            AS Pending,
                   SUM(status = 'Confirmed')           AS Confirmed,
                   SUM(status = 'Waiting')             AS Waiting,
                   SUM(status = 'Ongoing')             AS Ongoing,
                   SUM(status = 'Completed')           AS Completed,
                   SUM(status = 'Skipped')             AS Skipped,
                   SUM(status = 'Cancelled')           AS Cancelled,
                   SUM(status = 'Rejected')            AS Rejected,
                   SUM(status = 'NoShow')              AS NoShows
            FROM   appointments
            WHERE  provider_id      = @PId
              AND  appointment_date = CURDATE();";

        using var conn = _factory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<AppointmentStatsDto>(sql,
                   new { PId = providerId })
               ?? new AppointmentStatsDto();
    }

    // ── Customer ──────────────────────────────────────────────────────────────

    public async Task<List<CustomerAppointmentDto>> GetByCustomerAsync(uint customerId)
    {
        const string sql = @"
            SELECT a.id               AS Id,
                   a.booking_ref      AS BookingRef,
                   a.appointment_date AS AppointmentDate,
                   a.appointment_time AS AppointmentTime,
                   a.token_number     AS TokenNumber,
                   a.status           AS Status,
                   a.patient_name     AS PatientName,
                   a.visit_reason     AS VisitReason,
                   a.created_at       AS CreatedAt,
                   a.provider_id      AS ProviderId,
                   sp.firm_name       AS FirmName,
                   ps.name            AS ServiceName
            FROM   appointments          a
            JOIN   service_providers     sp ON sp.id = a.provider_id
            LEFT JOIN provider_services  ps ON ps.id = a.service_id
            WHERE  a.customer_id = @CId
            ORDER  BY a.appointment_date DESC, a.appointment_time;";

        using var conn = _factory.CreateConnection();
        return (await conn.QueryAsync<CustomerAppointmentDto>(sql,
                    new { CId = customerId })).ToList();
    }

    public async Task<string> CreateAsync(uint customerId, BookAppointmentDto dto)
    {
        using var conn = _factory.CreateConnection();
        conn.Open();
        using var tx = conn.BeginTransaction();

        try
        {
            // Lock the slot and read its details
            const string slotSql = @"
                SELECT id AS Id, provider_id AS ProviderId, service_id AS ServiceId,
                       slot_date AS SlotDate, start_time AS StartTime,
                       booked_count AS BookedCount, max_bookings AS MaxBookings,
                       is_active AS IsActive, is_blocked AS IsBlocked
                FROM provider_slots
                WHERE id = @SlotId
                FOR UPDATE;";

            var slot = await conn.QueryFirstOrDefaultAsync<SlotSnapshot>(
                           slotSql, new { SlotId = dto.SlotId }, tx);

            if (slot is null)                        throw new NotFoundException("Slot not found.");
            if (!slot.IsActive || slot.IsBlocked)    throw new AppException("Slot is not available.");
            if (slot.BookedCount >= slot.MaxBookings) throw new AppException("Slot is fully booked.");

            // Next token number for this provider today
            const string tokenSql = @"
                SELECT COALESCE(MAX(token_number), 0) + 1
                FROM   appointments
                WHERE  provider_id = @PId AND appointment_date = @Date;";

            var token = await conn.ExecuteScalarAsync<int>(tokenSql, new
            {
                PId  = slot.ProviderId,
                Date = slot.SlotDate.Date
            }, tx);

            // Insert with temporary booking_ref (will be replaced after we know the ID)
            var tempRef = $"#TEMP-{Guid.NewGuid():N}"[..20];
            const string insertSql = @"
                INSERT INTO appointments
                    (booking_ref, appointment_type, customer_id, provider_id, service_id,
                     slot_id, appointment_date, appointment_time, token_number,
                     patient_name, patient_age, visit_reason, status, created_at)
                VALUES
                    (@Ref, 'Slot', @CId, @PId, @SvcId,
                     @SlotId, @Date, @Time, @Token,
                     @PatientName, @PatientAge, @VisitReason, 'Pending', NOW());
                SELECT LAST_INSERT_ID();";

            var newId = await conn.ExecuteScalarAsync<uint>(insertSql, new
            {
                Ref         = tempRef,
                CId         = customerId,
                PId         = slot.ProviderId,
                SvcId       = slot.ServiceId,
                SlotId      = dto.SlotId,
                Date        = slot.SlotDate.Date,
                Time        = slot.StartTime,
                Token       = token,
                PatientName = dto.PatientName?.Trim(),
                PatientAge  = dto.PatientAge,
                VisitReason = dto.VisitReason?.Trim()
            }, tx);

            // Update booking_ref to final format: #BK00001
            var finalRef = $"#BK{newId:D5}";
            await conn.ExecuteAsync(
                "UPDATE appointments SET booking_ref = @Ref WHERE id = @Id;",
                new { Ref = finalRef, Id = newId }, tx);

            // Increment slot's booked_count
            await conn.ExecuteAsync(
                "UPDATE provider_slots SET booked_count = booked_count + 1 WHERE id = @Id;",
                new { Id = dto.SlotId }, tx);

            tx.Commit();
            return finalRef;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    public async Task<bool> CancelByCustomerAsync(uint id, uint customerId, string? reason)
    {
        using var conn = _factory.CreateConnection();
        conn.Open();
        using var tx = conn.BeginTransaction();

        try
        {
            const string readSql = @"
                SELECT slot_id AS SlotId, status AS Status
                FROM   appointments
                WHERE  id = @Id AND customer_id = @CId
                FOR UPDATE;";

            var appt = await conn.QueryFirstOrDefaultAsync<(uint? SlotId, string Status)>(
                           readSql, new { Id = id, CId = customerId }, tx);

            if (appt == default) return false;
            if (appt.Status is "Completed" or "Cancelled" or "Rejected" or "NoShow" or "Skipped")
                throw new AppException("This appointment cannot be cancelled.");

            await conn.ExecuteAsync(
                @"UPDATE appointments
                  SET    status              = 'Cancelled',
                         cancelled_by        = 'Customer',
                         cancellation_reason = @Reason,
                         updated_at          = NOW()
                  WHERE  id = @Id;",
                new { Id = id, Reason = reason }, tx);

            if (appt.SlotId.HasValue)
                await conn.ExecuteAsync(
                    "UPDATE provider_slots SET booked_count = GREATEST(0, booked_count - 1) WHERE id = @SId;",
                    new { SId = appt.SlotId.Value }, tx);

            tx.Commit();
            return true;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    // ── Private ───────────────────────────────────────────────────────────────

    private record SlotSnapshot(
        uint     Id,
        uint     ProviderId,
        uint?    ServiceId,
        DateTime SlotDate,
        TimeSpan StartTime,
        int      BookedCount,
        int      MaxBookings,
        bool     IsActive,
        bool     IsBlocked);
}

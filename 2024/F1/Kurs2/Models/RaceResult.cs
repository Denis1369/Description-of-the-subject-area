using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Kurs2;

public partial class RaceResult
{
    public uint Id { get; set; }

    public uint RaceId { get; set; }

    public uint DriverId { get; set; }

    public uint ConstructorId { get; set; }

    public string? StartPosition { get; set; }

    public string? FinishPosition { get; set; }

    public decimal? PointsAwarded { get; set; }

    public virtual Constructor Constructor { get; set; } = null!;

    public virtual Driver Driver { get; set; } = null!;

    public virtual Race Race { get; set; } = null!;

    public static string GenerateRacingResult(int seasonId, int driverId, int raceId, uint constructorId,
        string startPosition, string finishPosition)
    {
        if (seasonId <= 0)
            return "Не выбран сезон.";
        if (driverId <= 0)
            return "Не выбран пилот.";
        if (raceId <= 0)
            return "Не выбрана гонка.";
        if (constructorId == 0)
            return "Не выбрана команда.";

        if (string.IsNullOrWhiteSpace(startPosition))
            return "Не указана стартовая позиция.";
        if (string.IsNullOrWhiteSpace(finishPosition))
            return "Не указана финишная позиция.";

        try
        {

            if (!App._context.Seasons.Any(s => s.Id == (uint)seasonId))
                return "Указанный сезон не найден.";

            var race = App._context.Races.FirstOrDefault(r => r.Id == (uint)raceId);
            if (race == null)
                return "Указанная гонка не найдена.";
            if (race.SeasonId != (uint)seasonId)
                return "Выбранная гонка не относится к указанному сезону.";

            bool exists = App._context.RaceResults
                .Any(rr => rr.DriverId == (uint)driverId && rr.RaceId == (uint)raceId);
            if (exists)
                return "Результат для этого пилота в данной гонке уже существует.";

            string trimmedStart = startPosition.Trim();
            bool startTaken = App._context.RaceResults
                .Any(rr => rr.RaceId == (uint)raceId && rr.StartPosition == trimmedStart);
            if (startTaken)
                return $"Стартовая позиция {trimmedStart} уже занята другим пилотом.";

            string trimmedFinish = finishPosition.Trim();
            bool finishTaken = App._context.RaceResults
                .Any(rr => rr.RaceId == (uint)raceId && rr.FinishPosition == trimmedFinish);
            if (finishTaken)
                return $"Финишная позиция {trimmedFinish} уже занята другим пилотом.";

            var result = new RaceResult
            {
                DriverId = (uint)driverId,
                RaceId = (uint)raceId,
                ConstructorId = constructorId,
                StartPosition = trimmedStart,
                FinishPosition = trimmedFinish,
                PointsAwarded = CalculatePoints(trimmedFinish)
            };

            App._context.RaceResults.Add(result);
            App._context.SaveChanges();

            return "Результат успешно добавлен.";
        }
        catch (Exception ex)
        {
            return $"При сохранении в БД: {ex.Message}";
        }
    }

    public static string DeleteRacingResult(int seasonId, int driverId, int raceId)
    {
        if (seasonId <= 0)
            return "Не выбран сезон.";
        if (driverId <= 0)
            return "Не выбран пилот.";
        if (raceId <= 0)
            return "Не выбрана гонка.";

        try
        {
            if (!App._context.Seasons.Any(s => s.Id == (uint)seasonId))
                return "Указанный сезон не найден.";

            var race = App._context.Races
                .FirstOrDefault(r => r.Id == (uint)raceId && r.SeasonId == (uint)seasonId);

            if (race == null)
                return "Указанная гонка не найдена или не принадлежит сезону.";

            var result = App._context.RaceResults
                .FirstOrDefault(rr =>
                    rr.DriverId == (uint)driverId &&
                    rr.RaceId == (uint)raceId);

            if (result == null)
                return "Результат для удаления не найден.";

            App._context.RaceResults.Remove(result);
            App._context.SaveChanges();

            return "Результат успешно удалён.";
        }
        catch (Exception ex)
        {
            return $"При удалении из БД: {ex.Message}";
        }
    }



    private static decimal CalculatePoints(string finishPosition)
    {
        return finishPosition switch
        {
            "1" => 25,
            "2" => 18,
            "3" => 15,
            "4" => 12,
            "5" => 10,
            "6" => 8,
            "7" => 6,
            "8" => 4,
            "9" => 2,
            "10" => 1,
            _ => 0
        };
    }
}

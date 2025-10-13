using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kurs2;

public partial class Race
{
    public uint Id { get; set; }

    public uint SeasonId { get; set; }

    public uint CircuitId { get; set; }

    public int? Round { get; set; }

    public string? RaceName { get; set; }

    public DateOnly RaceDate { get; set; }

    [Column(TypeName = "ENUM('Запланировано','Завершено','Отменено','Отложено')")]
    public string? RaceStatus { get; set; }

    public virtual Circuit Circuit { get; set; } = null!;

    public virtual ICollection<RaceResult> RaceResults { get; set; } = new List<RaceResult>();

    public virtual Season Season { get; set; } = null!;

    public static string GeneratRace(int season ,uint circuitId, DateOnly raceDate, string raceName)
    {
        try
        {
            using (var context = new Formula12025Context())
            {
                var lastRace = context.Races
                    .OrderByDescending(r => r.Id)
                    .FirstOrDefault();

                int nextRound = (lastRace?.Round ?? 0) + 1;

                var race = new Race
                {
                    SeasonId = (uint)season,
                    CircuitId = circuitId,
                    RaceDate = raceDate,
                    RaceName = raceName,
                    Round = nextRound,
                    RaceStatus = "scheduled"
                };

                context.Races.Add(race);
                context.SaveChanges();
                return "Гонка добавлена";
            }
        }
        catch (Exception ex)
        {
            return $"Ошибка: {ex.Message}";
        }
    }
}

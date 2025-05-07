using Microsoft.EntityFrameworkCore;
using Npgsql;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Repository.Implements
{
    public class TournamentRepository : ITournamentRepository
    {
        private readonly StrateZoneDbContext _context;

        public TournamentRepository(StrateZoneDbContext context)
        {
            _context = context;
        }

        /*
        public async Task<Tournament> DeleteTournamentAsync(int id)
        {
            try
            {
                var deletingObject = await _context.Tournaments.FindAsync(id);
                _context.Tournaments.Remove(deletingObject);
                await _context.SaveChangesAsync();

                return deletingObject;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Tournament>> GetAllAsync()
        {
            try
            {
                return await _context.Tournaments.AsNoTracking().ToListAsync();
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Tournament> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Tournaments.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Tournament> CreateTournamentAsync(Tournament tournament)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();

                if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync();

                await using var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO tournaments (user_id, name, description, targeted_ranking, max_participant, start_date, end_date, status) 
                    VALUES (@user_id, @name, @description, @targeted_ranking, @max_participant, @start_date, @end_date, @tournament_status::tournament_status)
                    RETURNING room_id;"
                ;

                cmd.Parameters.Add(new NpgsqlParameter("@user_id", tournament.UserId));
                cmd.Parameters.Add(new NpgsqlParameter("@name", tournament.Name));
                cmd.Parameters.Add(new NpgsqlParameter("@description", tournament.Description));
                cmd.Parameters.Add(new NpgsqlParameter("@targeted_ranking", tournament.TargetedRanking));
                cmd.Parameters.Add(new NpgsqlParameter("@max_participant", tournament.MaxParticipants));
                cmd.Parameters.Add(new NpgsqlParameter("@start_date", tournament.StartDate));
                cmd.Parameters.Add(new NpgsqlParameter("@end_date", tournament.EndDate));
                cmd.Parameters.Add(new NpgsqlParameter("@tournament_status", tournament.Status.ToString()));

                var newTournamentId = await cmd.ExecuteScalarAsync();
                tournament.TournamentId= Convert.ToInt32(newTournamentId);

                return tournament;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Tournament> UpdateTournamentAsync(int id, Tournament tournament)
        {
            try
            {
                var existingTournament = await _context.Tournaments.FindAsync(id) ?? throw new Exception("Room with this ID does not exist");

                _context.Entry(existingTournament).State = EntityState.Detached;

                existingTournament.TournamentId = id;

                var parameters = new List<NpgsqlParameter>();
                var sql = new StringBuilder("UPDATE tournaments SET ");

                if (!string.IsNullOrEmpty(tournament.UserId.ToString()))
                {
                    sql.Append("user_id = @user_id, ");
                    parameters.Add(new NpgsqlParameter("@user_id", tournament.UserId));
                }

                if (!string.IsNullOrEmpty(tournament.Name.ToString()))
                {
                    sql.Append("name = @name, ");
                    parameters.Add(new NpgsqlParameter("@name", tournament.Name));
                }

                if (tournament.MaxParticipants.HasValue)
                {
                    sql.Append("capacity = @capacity, ");
                    parameters.Add(new NpgsqlParameter("@capacity", tournament.MaxParticipants));
                }

                if (!string.IsNullOrEmpty(tournament.Description))
                {
                    sql.Append("description = @description, ");
                    parameters.Add(new NpgsqlParameter("@description", tournament.Description));
                }

                sql.Append("tournament_status = @tournament_status::tournament_status, ");
                parameters.Add(new NpgsqlParameter("@tournament_status", tournament.Status.ToString()));

                if (tournament.StartDate.HasValue)
                {
                    sql.Append("start_date = @start_date::start_date, ");
                    parameters.Add(new NpgsqlParameter("@start_date", tournament.StartDate));
                }

                if (tournament.EndDate.HasValue)
                {
                    sql.Append("end_date = @end_date::end_date, ");
                    parameters.Add(new NpgsqlParameter("@end_date", tournament.EndDate));
                }

                if (tournament.UpdatedAt.HasValue)
                {
                    sql.Append("updated_at = @updated_at::updated_at, ");
                    parameters.Add(new NpgsqlParameter("@updated_at", tournament.UpdatedAt));
                }


                sql.Remove(sql.Length - 2, 2);
                sql.Append(" WHERE room_id = @id");
                parameters.Add(new NpgsqlParameter("@id", id));

                await _context.Database.ExecuteSqlRawAsync(sql.ToString(), parameters.ToArray());

                var updatedTournament = await _context.Tournaments.FindAsync(id);
                return updatedTournament;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        */
    }
}

// ============================================================================
// Eldritch Adventures Aide
// Arkayn Game Designs
// Copyright (c) 2026 Arkayn Game Designs. All Rights Reserved.
// ============================================================================
// File:        LookupRepository.cs
// Purpose:     Contains the data access logic for Lookups
// Author:      Erik Luken
// Created:     26 JULY 2026
// ============================================================================
// NOTICE: This software and all associated content are the exclusive property
//         of Arkayn Game Designs. Unauthorized use, reproduction, or
//         distribution is strictly prohibited.
// ============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using ArkaynDAL;
using ArkaynDAL.Core;
using ArkaynDAL.Interfaces;
using EACore_Data.Models;

/// <summary>
///       Namespace Name - EA_Aide.DataAccess.
/// </summary>
namespace EA_Aide.DataAccess
{
    public class PlayerGameRepository : RepositoryBase
    {

        /// <summary>
        ///  Function Name :  PlayerGameRepository.
        /// </summary>
        /// <param name="conn">This conn's Datatype is : ArkaynDAL.Interfaces.IArkaynConnection.</param>
        /// <returns>void.</returns>
        public PlayerGameRepository(IArkaynConnection conn) : base(conn)
        {
        }


        /// <summary>
        ///  Function Name :  GetAllPlayerGamesAsync.
        /// </summary>
        /// <returns>System.Threading.Tasks.Task<System.Collections.Generic.List<EACore_Data.Models.PlayerGame>>.</returns>
        public async Task<List<PlayerGame>> GetAllPlayerGamesAsync()
        {
            var playerGames = new List<PlayerGame>();

            using var reader = await _conn.ExecuteReaderAsync("SELECT * FROM PlayerGame");
            while (reader.Read())
            {
                playerGames.Add(MapPlayerGame(reader));
            }

            return playerGames;
        }


        /// <summary>
        ///  Function Name :  GetPlayerGameByIdAsync.
        /// </summary>
        /// <param name="playergameId">This playergameId's Datatype is : int.</param>
        /// <returns>System.Threading.Tasks.Task<EACore_Data.Models.PlayerGame>.</returns>
        public async Task<PlayerGame> GetPlayerGameByIdAsync(int playergameId)
        {
            using var reader = await _conn.ExecuteReaderAsync(
                "SELECT * FROM PlayerGame WHERE PlayerGameID=@id",
                new ArkaynParameter("@id", playergameId)
            );

            if (reader.Read())
                return MapPlayerGame(reader);

            return null;
        }


        /// <summary>
        ///  Function Name :  AddPlayerGameAsync.
        /// </summary>
        /// <param name="playergame">This playergame's Datatype is : EACore_Data.Models.PlayerGame.</param>
        /// <returns>System.Threading.Tasks.Task<int>.</returns>
        public async Task<int> AddPlayerGameAsync(PlayerGame playergame)
        {
            var sql = @"
                INSERT INTO PlayerGame (
                    PlayerCodexID, PlayerID, GameID, Notes, CreatedOn, LastUpdated
                )
                VALUES (
                    @PlayerCodexID, @PlayerID, @GameID, @Notes, @CreatedOn, @LastUpdated
                );
                SELECT last_insert_rowid();
            ";

            return await _conn.ExecuteScalarAsync<int>(
                sql,
                BuildParameters(playergame)
            );
        }


        /// <summary>
        ///  Function Name :  UpdatePlayerGamAsync.
        /// </summary>
        /// <param name="playerGame">This playerGame's Datatype is : EACore_Data.Models.PlayerGame.</param>
        /// <returns>System.Threading.Tasks.Task.</returns>
        public async Task UpdatePlayerGamAsync(PlayerGame playerGame)
        {
            var sql = @"
                UPDATE Games SET
                    PlayerCodexID=@PlayerCodexID,
                    PlayerID=@PlayerID,
                    GameID=@GameID,
                    Notes=@Notes,
                    CreatedOn=@CreatedOn,
                    LastUpdated=@LastUpdated,
                WHERE PlayerGameID=@PlayerGameID
            ";

            await _conn.ExecuteNonQueryAsync(sql, BuildParameters(playerGame));
        }


        /// <summary>
        ///  Function Name :  DeletePlayerGameAsync.
        /// </summary>
        /// <param name="playergameId">This playergameId's Datatype is : int.</param>
        /// <returns>System.Threading.Tasks.Task.</returns>
        public async Task DeletePlayerGameAsync(int playergameId)
        {
            await _conn.ExecuteNonQueryAsync(
                "DELETE FROM PlayerGame WHERE PlayerGameID=@id",
                new ArkaynParameter("@id", playergameId)
            );
        }


        /// <summary>
        ///  Function Name :  BuildParameters.
        /// </summary>
        /// <param name="playerGame">This playerGame's Datatype is : EACore_Data.Models.PlayerGame.</param>
        /// <param name="includeId">This includeId's Datatype is : bool.</param>
        /// <returns>ArkaynDAL.Core.ArkaynParameter[].</returns>
        private ArkaynParameter[] BuildParameters(PlayerGame playerGame, bool includeId = false)
        {
            var list = new List<ArkaynParameter>
            {
                new ArkaynParameter("@PlayerCodexID", playerGame.PlayerCodexID),
                new ArkaynParameter("@PlayerID", playerGame.PlayerID),
                new ArkaynParameter("@GameID", playerGame.GameID),
                new ArkaynParameter("Notes", playerGame.Notes),
                new ArkaynParameter("@CreatedOn", playerGame.CreatedOn),
                new ArkaynParameter("@LastUpdated", playerGame.LastUpdated)
            };

            if (includeId)
                list.Add(new ArkaynParameter("@PlayerGameID", playerGame.PlayerGameID));

            return list.ToArray();
        }


        /// <summary>
        ///  Function Name :  MapPlayerGame.
        /// </summary>
        /// <param name="reader">This reader's Datatype is : ArkaynDAL.Interfaces.IArkaynReader.</param>
        /// <returns>EACore_Data.Models.PlayerGame.</returns>
        private PlayerGame MapPlayerGame(IArkaynReader reader)
        {
            var playergame = new PlayerGame
            {
                PlayerGameID = reader.GetInt32("PlayerGameID"),
                PlayerCodexID = reader.GetInt32("PlayerCodexID"),
                PlayerID = reader.GetInt32("PlayerID"),
                GameID = reader.GetInt32("GameID"),
                Notes = reader.GetNullableString("Notes"),
                CreatedOn = reader.GetDateTime("CreatedOn"),
                LastUpdated = reader.GetNullableDateTime("LastUpdated"),
            };

            return playergame;
        }


    }
}

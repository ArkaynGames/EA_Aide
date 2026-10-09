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

using System.Collections.Generic;
using System.Threading.Tasks;
using ArkaynDAL;
using ArkaynDAL.Core;
using ArkaynDAL.Interfaces;
using EACore_Data.Models;

namespace EA_Aide.DataAccess
{
    public class LookupRepository : RepositoryBase
    {
        public LookupRepository(IArkaynConnection conn) : base(conn)
        {
        }

        // ---------------------------------------------------------
        // GET ALL LOOKUPS
        // ---------------------------------------------------------
        public async Task<List<Lookup>> GetAllAsync()
        {
            var list = new List<Lookup>();

            using var reader = await _conn.ExecuteReaderAsync("SELECT * FROM Lookups");

            while (reader.Read())
                list.Add(MapLookup(reader));

            return list;
        }

        // ---------------------------------------------------------
        // GET BY CATEGORY
        // ---------------------------------------------------------
        public async Task<List<Lookup>> GetByCategoryAsync(int categoryId)
        {
            var list = new List<Lookup>();

            using var reader = await _conn.ExecuteReaderAsync(
                "SELECT * FROM Lookups WHERE CategoryID=@cat",
                new ArkaynParameter("@cat", categoryId)
            );

            while (reader.Read())
                list.Add(MapLookup(reader));

            return list;
        }

        // ---------------------------------------------------------
        // GET BY CATEGORY + LINKEDID
        // ---------------------------------------------------------
        public async Task<Lookup?> GetAsync(int categoryId, int linkedId)
        {
            using var reader = await _conn.ExecuteReaderAsync(
                "SELECT * FROM Lookups WHERE CategoryID=@cat AND LinkedID=@lid",
                new ArkaynParameter("@cat", categoryId),
                new ArkaynParameter("@lid", linkedId)
            );

            if (reader.Read())
                return MapLookup(reader);

            return null;
        }

        // ---------------------------------------------------------
        // ADD LOOKUP
        // ---------------------------------------------------------
        public async Task<int> AddAsync(Lookup lookup)
        {
            var sql = @"
                INSERT INTO Lookups (
                    CategoryID, LinkedID, Description, CategoryName, Value, Effect
                )
                VALUES (
                    @CategoryID, @LinkedID, @Description, @CategoryName, @Value, @Effect
                );
                SELECT last_insert_rowid();
            ";

            return await _conn.ExecuteScalarAsync<int>(
                sql,
                BuildParameters(lookup, includeId: false)
            );
        }

        // ---------------------------------------------------------
        // UPDATE LOOKUP
        // ---------------------------------------------------------
        public async Task UpdateAsync(Lookup lookup)
        {
            var sql = @"
                UPDATE Lookups SET
                    CategoryID=@CategoryID,
                    LinkedID=@LinkedID,
                    Description=@Description,
                    CategoryName=@CategoryName,
                    Value=@Value,
                    Effect=@Effect
                WHERE LookupID=@LookupID
            ";

            await _conn.ExecuteNonQueryAsync(
                sql,
                BuildParameters(lookup, includeId: true)
            );
        }

        // ---------------------------------------------------------
        // DELETE LOOKUP
        // ---------------------------------------------------------
        public async Task DeleteAsync(int lookupId)
        {
            await _conn.ExecuteNonQueryAsync(
                "DELETE FROM Lookups WHERE LookupID=@id",
                new ArkaynParameter("@id", lookupId)
            );
        }

        // ---------------------------------------------------------
        // PARAMETER BUILDER
        // ---------------------------------------------------------
        private ArkaynParameter[] BuildParameters(Lookup lookup, bool includeId)
        {
            var list = new List<ArkaynParameter>
            {
                new ArkaynParameter("@CategoryID", lookup.CategoryID),
                new ArkaynParameter("@LinkedID", lookup.LinkedID),
                new ArkaynParameter("@Description", lookup.Description),
                new ArkaynParameter("@CategoryName", lookup.CategoryName),
                new ArkaynParameter("@Value", lookup.Value),
                new ArkaynParameter("@Effect", lookup.Effect)
            };

            if (includeId)
                list.Add(new ArkaynParameter("@LookupID", lookup.LookupID));

            return list.ToArray();
        }

        // ---------------------------------------------------------
        // MAPPING: DB → MODEL
        // ---------------------------------------------------------
        private Lookup MapLookup(IArkaynReader reader)
        {
            return new Lookup
            {
                LookupID = reader.GetInt32("LookupID"),
                CategoryID = reader.GetInt32("CategoryID"),
                LinkedID = reader.GetInt32("LinkedID"),
                Description = reader.GetString("Description"),
                CategoryName = reader.GetString("CategoryName"),
                Value = reader.IsDBNull("Value") ? null : reader.GetInt32("Value"),
                Effect = reader.GetNullableString("Effect")
            };
        }
    }
}

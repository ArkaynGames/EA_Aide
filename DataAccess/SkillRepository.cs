// ============================================================================
// Eldritch Adventures
// Arkayn Game Designs
// Copyright (c) 2026 Arkayn Game Designs. All Rights Reserved.
// ============================================================================
// File:        SkillRepository.cs
// Purpose:     Contains the data access logic for Skills
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
    public class SkillRepository : RepositoryBase
    {
        public SkillRepository(IArkaynConnection conn) : base(conn)
        {
        }

        // ---------------------------------------------------------
        // GET ALL SkillS
        // ---------------------------------------------------------
        public async Task<List<Skill>> GetAllAsync()
        {
            var list = new List<Skill>();

            using var reader = await _conn.ExecuteReaderAsync("SELECT * FROM Skills");

            while (reader.Read())
                list.Add(MapSkill(reader));

            return list;
        }

        // ---------------------------------------------------------
        // GET BY CATEGORY
        // ---------------------------------------------------------
        public async Task<List<Skill>> GetByCategoryAsync(int categoryId)
        {
            var list = new List<Skill>();

            using var reader = await _conn.ExecuteReaderAsync(
                "SELECT * FROM Skills WHERE CategoryID=@cat",
                new ArkaynParameter("@cat", categoryId)
            );

            while (reader.Read())
                list.Add(MapSkill(reader));

            return list;
        }

        // ---------------------------------------------------------
        // ADD skill
        // ---------------------------------------------------------
        public async Task<int> AddAsync(Skill skill)
        {
            var sql = @"
                INSERT INTO Skills (
                    SkillName, Stat1, Stat2, CategoryID, SkillType
                )
                VALUES (
                    @SkillName, @Stat1, @Stat2, @CategoryID, @SkillType
                );
                SELECT last_insert_rowid();
            ";

            return await _conn.ExecuteScalarAsync<int>(
                sql,
                BuildParameters(skill, includeId: false)
            );
        }

        // ---------------------------------------------------------
        // UPDATE skill
        // ---------------------------------------------------------
        public async Task UpdateAsync(Skill Skill)
        {
            var sql = @"
                UPDATE Skills SET
                    SkillName=@SkillName,
                    Stat1=@Stat1,
                    Stat2=@Stat2,
                    CategoryID=@CategoryID
                    SkillType=@SkillType
                WHERE SkillID=@SkillID
            ";

            await _conn.ExecuteNonQueryAsync(
                sql,
                BuildParameters(Skill, includeId: true)
            );
        }

        // ---------------------------------------------------------
        // DELETE skill
        // ---------------------------------------------------------
        public async Task DeleteAsync(int SkillId)
        {
            await _conn.ExecuteNonQueryAsync(
                "DELETE FROM Skills WHERE SkillID=@id",
                new ArkaynParameter("@id", SkillId)
            );
        }

        // ---------------------------------------------------------
        // PARAMETER BUILDER
        // ---------------------------------------------------------
        private ArkaynParameter[] BuildParameters(Skill skill, bool includeId)
        {
            var list = new List<ArkaynParameter>
            {
                new ArkaynParameter("@CategoryID", skill.CategoryID),
                new ArkaynParameter("@SkillName", skill.SkillName),
                new ArkaynParameter("@Stat1", skill.Stat1),
                new ArkaynParameter("@Stat2", skill.Stat2),
                new ArkaynParameter("@SkillType", skill.SkillType)
            };

            if (includeId)
                list.Add(new ArkaynParameter("@SkillID", skill.SkillID));

            return list.ToArray();
        }

        // ---------------------------------------------------------
        // MAPPING: DB → MODEL
        // ---------------------------------------------------------
        private Skill MapSkill(IArkaynReader reader)
        {
            return new Skill
            {
                SkillID = reader.GetInt32("SkillID"),
                CategoryID = reader.GetInt32("CategoryID"),
                SkillName = reader.GetString("SkillName"),
                Stat1 = reader.GetString("Stat1"),
                Stat2 = reader.GetString("Stat2"),
                SkillType = reader.GetInt32("SkillType")

            };
        }
    }
}

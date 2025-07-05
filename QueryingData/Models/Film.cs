using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryingData.Models
{
    public enum MpaaRating
    {
        G,
        PG,
        PG13,   // Use PG13 for 'PG-13' (hyphens not allowed in enum names)
        R,
        NC17    // Use NC17 for 'NC-17'
    }

    public class Film
    {
        public int FilmId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int ReleaseYear { get; set; }
        public int LanguageId { get; set; }
        public int RentalDuration { get; set; }
        public decimal RentalRate { get; set; }
        public int Length { get; set; }
        public decimal ReplacementCost { get; set; }
        public string Rating { get; set; }  // public MpaaRating Rating { get; set; }
        public DateTime LastUpdate { get; set; }
        public string? SpecialFeatures { get; set; }
        public NpgsqlTsVector? FullText { get; set; }
    }

    public class FilmWithCategory : Film
    {
        public int FilmId { get; set; }
        public string? Title { get; set; } = string.Empty;
        public string? Category { get; set; }
    }
}
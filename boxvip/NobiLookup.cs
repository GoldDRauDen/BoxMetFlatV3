using System;
using System.Collections.Generic;

namespace BoxMetPlugin
{
    // =====================================================================
    //  NOBI LOOKUP â€” tra Ä‘á»™ dÃ£n dÃ i uá»‘n tham kháº£o theo nobi-standards.md
    //  SS/MS â†’ báº£ng 1 | SUS â†’ báº£ng 2 (cá»™t 2B) | AL â†’ báº£ng 3
    //  Chiá»u dÃ i uá»‘n = max(L, W)
    // =====================================================================
    public class NobiResult
    {
        public bool Found;
        public double Nobi;
        public string V = "";
        public string RangeText = "";
        public string ConfirmNote = "";
        public string Message = "";
    }

    public static class NobiLookup
    {
        readonly struct Row
        {
            public readonly double T, LMin, LMax;
            public readonly string V, Range;
            public readonly double? Nobi;
            public readonly bool Confirm;
            public Row(double t, double lmin, double lmax,
                       string v, string range, double? nobi, bool confirm = false)
            {
                T = t; LMin = lmin; LMax = lmax;
                V = v; Range = range; Nobi = nobi; Confirm = confirm;
            }
        }

        // Báº£ng 1: SS, SUS430, Ä‘á»“ng thau â€” MS dÃ¹ng chung
        static readonly List<Row> T_SS = new List<Row>
        {
            new Row(0.5, 0, 2400, "V8", "â‰¤2400", 1.36),
            new Row(0.8, 0, 2400, "V8", "â‰¤2400", 1.68),
            new Row(1.0, 0, 2400, "V8", "â‰¤2400", 1.90),
            new Row(1.2, 0, 2400, "V8", "â‰¤2400", 2.14),
            new Row(1.5, 0, 2400, "V8", "â‰¤2400", 2.54),
            new Row(1.6, 0, 2400, "V8", "â‰¤2400", 2.64),
            new Row(2.0, 0, 2400, "V12", "â‰¤2400", 3.44),
            new Row(2.3, 0, 2400, "V12", "â‰¤2400", 3.80),
            new Row(3.0, 0, 1000, "V12", "â‰¤1000", 4.66),
            new Row(3.0, 1001, 1900, "V18", "1001~1900", 5.28),
            new Row(3.0, 1901, 2400, "V25", "1901~2400", 5.98),
            new Row(3.2, 0, 1800, "V18", "â‰¤1800", 5.50),
            new Row(3.2, 1801, 2400, "V25", "1801~2400", 5.72),
            new Row(4.0, 0, 1200, "V18", "â‰¤1200", 6.42),
            new Row(4.5, 0, 1200, "V25", "â‰¤1200", 7.76),
            new Row(4.5, 1201, 1600, "V32", "1201~1600", 8.10),
            new Row(4.5, 1601, 2300, "V40", "1601~2300", 8.48),
            new Row(4.5, 0, double.MaxValue, "V18", "má»i L", 7.02, true),
            new Row(5.0, 0, 1000, "V25", "â‰¤1000", 8.32),
            new Row(5.0, 1001, 1300, "V32", "1001~1300", 9.04),
            new Row(5.0, 1301, 1600, "V40", "1301~1600", 9.86),
            new Row(6.0, 0, 700, "V25", "â‰¤700", 9.78),
            new Row(6.0, 701, 900, "V32", "701~900", 10.22),
            new Row(6.0, 901, 1150, "V40", "901~1150", 10.94),
            new Row(9.0, 0, 400, "V40", "â‰¤400", 14.62),
            new Row(9.0, 401, 800, "V80", "401~800", 15.50),
            new Row(12.0, 0, 550, "V80", "â‰¤550", 20.48),
        };

        // Báº£ng 2: SUS (trá»« SUS430) â€” cá»™t 2B
        static readonly List<Row> T_SUS = new List<Row>
        {
            new Row(0.5, 0, 2400, "V8", "â‰¤2400", 1.36),
            new Row(0.8, 0, 2400, "V8", "â‰¤2400", 1.82),
            new Row(1.0, 0, 2400, "V8", "â‰¤2400", 2.14),
            new Row(1.2, 0, 2400, "V8", "â‰¤2400", 2.32),
            new Row(1.5, 0, 2400, "V8", "â‰¤2400", 2.64),
            new Row(2.0, 0, 1300, "V12", "â‰¤1300", 3.68),
            new Row(2.0, 1301, 2400, "V18", "1301~2400", 4.00),
            new Row(2.5, 0, 1300, "V12", "â‰¤1300", 4.24),
            new Row(2.5, 1301, 2000, "V18", "1301~2000", 5.06),
            new Row(2.5, 2001, 2400, "V25", "2001~2400", 5.84),
            new Row(3.0, 0, 750, "V12", "â‰¤750", 4.80),
            new Row(3.0, 751, 1250, "V18", "751~1250", 5.64),
            new Row(3.0, 1251, 1800, "V25", "1251~1800", 6.46),
            new Row(3.0, 1801, 2400, "V40", "1801~2400", 8.54),
            new Row(4.0, 0, 650, "V18", "â‰¤650", 6.78),
            new Row(4.0, 651, 1000, "V25", "651~1000", 7.66),
            new Row(4.0, 1001, 1650, "V40", "1001~1650", 8.44),
            new Row(4.5, 0, 550, "V18", "â‰¤550", 7.30),
            new Row(5.0, 0, 550, "V25", "â‰¤550", 8.82),
            new Row(5.0, 551, 750, "V32", "551~750", 9.68),
            new Row(5.0, 751, 1000, "V40", "751~1000", 10.74),
            new Row(6.0, 0, 500, "V32", "â‰¤500", 10.72),
            new Row(6.0, 501, 650, "V40", "501~650", 11.88),
            new Row(8.0, 0, 350, "V40", "â‰¤350", 14.06),
            new Row(8.0, 351, 700, "V80", "351~700", 19.46),
            new Row(9.0, 0, 550, "V80", "â‰¤550", 20.54),
        };

        // Báº£ng 3: NhÃ´m (AL)
        static readonly List<Row> T_AL = new List<Row>
        {
            new Row(0.8, 0, 2400, "V8", "â‰¤2400", 1.84),
            new Row(1.0, 0, 2400, "V8", "â‰¤2400", 1.90),
            new Row(1.5, 0, 2400, "V8", "â‰¤2400", 2.40),
            new Row(2.0, 0, 2400, "V12", "â‰¤2400", 3.20),
            new Row(3.0, 0, 1200, "V12", "â‰¤1200", 4.56),
            new Row(3.0, 1201, 1900, "V18", "1201~1900", 4.78),
            new Row(3.0, 1901, 2400, "V25", "1901~2400", null, true),
            new Row(4.0, 0, 1200, "V18", "â‰¤1200", 6.10),
            new Row(4.0, 1201, 1700, "V25", "1201~1700", 6.42),
            new Row(4.0, 1701, 2000, "V32", "1701~2000", 6.84),
            new Row(4.0, 2001, 2400, "V40", "2001~2400", 7.40),
            new Row(5.0, 0, 1000, "V25", "â‰¤1000", 8.26),
            new Row(5.0, 1001, 1300, "V32", "1001~1300", 8.44),
            new Row(5.0, 1301, 1600, "V40", "1301~1600", 8.74),
            new Row(6.0, 0, 700, "V25", "â‰¤700", 9.62),
            new Row(6.0, 701, 900, "V32", "701~900", 9.80),
            new Row(6.0, 901, 1150, "V40", "901~1150", 10.06),
            new Row(8.0, 0, 650, "V40", "â‰¤650", 12.78),
            new Row(8.0, 651, 800, "V80", "651~800", 14.90),
            new Row(9.0, 0, 400, "V40", "â‰¤400", 14.12),
            new Row(9.0, 401, 800, "V80", "401~800", 16.20),
            new Row(10.0, 0, 800, "V80", "â‰¤800", 17.52),
        };

        public static NobiResult Lookup(string material, double thickness, double bendLen)
        {
            string m = (material ?? "").Trim().ToUpper();
            List<Row> table = m == "SUS" ? T_SUS : m == "AL" ? T_AL : T_SS;
            string tblName = m == "SUS" ? "SUS-2B" : m == "AL" ? "AL" : "SS/MS";

            bool anyT = false;
            foreach (var r in table)
            {
                if (Math.Abs(r.T - thickness) > 1e-9) continue;
                anyT = true;
                if (bendLen >= r.LMin - 1e-9 && bendLen <= r.LMax + 1e-9)
                {
                    if (!r.Nobi.HasValue)
                        return new NobiResult
                        {
                            Found = false,
                            V = r.V,
                            RangeText = r.Range,
                            Message = $"T={thickness} / L uá»‘n {r.Range}: báº£ng ghi 'cáº§n xÃ¡c nháº­n'"
                        };
                    return new NobiResult
                    {
                        Found = true,
                        Nobi = r.Nobi.Value,
                        V = r.V,
                        RangeText = r.Range,
                        ConfirmNote = r.Confirm ? " â€” cáº§n xÃ¡c nháº­n rÃ£nh V" : ""
                    };
                }
            }
            if (!anyT)
                return new NobiResult
                {
                    Found = false,
                    Message = $"T={thickness} khÃ´ng cÃ³ trong báº£ng {tblName}"
                };
            return new NobiResult
            {
                Found = false,
                Message = $"L uá»‘n {bendLen:0.#} vÆ°á»£t báº£ng {tblName} (T={thickness})"
            };
        }
    }
}


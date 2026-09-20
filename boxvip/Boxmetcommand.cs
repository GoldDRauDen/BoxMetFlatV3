using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using AcColor = Autodesk.AutoCAD.Colors.Color;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

// =====================================================================
//  BOXMET AutoCAD Plugin â€” AutoCAD 2023/2024
//  Lá»‡nh: BOXMET
//  Váº½ tráº£i pháº³ng há»™p kim loáº¡i táº¥m trá»±c tiáº¿p lÃªn báº£n váº½ AutoCAD
//  Theo Mau_test.dxf: toÃ n bá»™ layer 0, base DASHED, slit mÃ u 3
// =====================================================================

[assembly: CommandClass(typeof(BoxMetPlugin.BoxMetCommand))]

namespace BoxMetPlugin
{
    // â”€â”€â”€ PARAMETERS MODEL â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    public class BoxParams
    {
        public double Length { get; set; } = 200;
        public double Width { get; set; } = 150;
        public double Thickness { get; set; } = 1.5;
        public string Material { get; set; } = "SS";
        public double Nobi { get; set; } = 0;
        public double H_Bottom { get; set; } = 30;
        public double H_Right { get; set; } = 30;
        public double H_Top { get; set; } = 30;
        public double H_Left { get; set; } = 30;
        public Point3d BasePoint { get; set; } = Point3d.Origin;

        public double SlitLen => Thickness + 0.2;
        public double LeadVal => (Material.ToUpper() == "SS" && Thickness < 6.0) ? 3.0 : 6.0;
        public bool IsLongX => Length >= Width;
    }

    // â”€â”€â”€ GEOMETRY HELPER â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    static class Geo
    {
        public static Point3d P(double x, double y) => new Point3d(x, y, 0);
        public static Point3d Offset(Point3d p, double ox, double oy) => new Point3d(p.X + ox, p.Y + oy, 0);
    }

    // â”€â”€â”€ DRAWING ENGINE â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    public static class DrawEngine
    {
        public const string LAYER = "0";
        public const short COLOR_SLIT = 3;

        static Line AddLine(BlockTableRecord btr, Transaction tr,
            Point3d a, Point3d b,
            bool dashed = false, short colorIdx = 0)
        {
            var ln = new Line(a, b)
            {
                Layer = LAYER,
                Linetype = "BYLAYER"
            };
            // NOBI is the only geometry that overrides the layer linetype.
            if (dashed) ln.Linetype = "DASHED";
            if (colorIdx > 0)
                ln.Color = AcColor.FromColorIndex(ColorMethod.ByAci, colorIdx);
            btr.AppendEntity(ln);
            tr.AddNewlyCreatedDBObject(ln, true);
            return ln;
        }

        static void AddArc(BlockTableRecord btr, Transaction tr,
                           Point3d center, double radius,
                           double startDeg, double endDeg, short colorIdx = 0)
        {
            var arc = new Arc(center, radius,
                              startDeg * Math.PI / 180.0,
                              endDeg * Math.PI / 180.0)
            {
                Layer = LAYER,
                Linetype = "BYLAYER"
            };
            if (colorIdx > 0)
                arc.Color = AcColor.FromColorIndex(ColorMethod.ByAci, colorIdx);
            btr.AppendEntity(arc);
            tr.AddNewlyCreatedDBObject(arc, true);
        }

        // â”€â”€ SLIT + LEAD-IN + FILLET ARC â”€â”€
        static void DrawSlit(BlockTableRecord btr, Transaction tr,
                     Point3d baseP, Point3d slitEnd, Point3d leadEnd,
                     double filletR = 0.5)
        {
            try
            {
                var u1 = (slitEnd - baseP);
                var u2 = (leadEnd - baseP);
                double len1 = u1.Length;
                double len2 = u2.Length;
                if (len1 < 0.01 || len2 < 0.01) return;

                var n1 = u1.GetNormal();
                var n2 = u2.GetNormal();

                double cosA = Math.Max(-1.0, Math.Min(1.0, n1.DotProduct(n2)));
                double angle = Math.Acos(cosA);
                if (angle < 0.01 || angle > Math.PI - 0.01) return;

                double tanDist = filletR / Math.Tan(angle / 2.0);
                double centerDist = filletR / Math.Sin(angle / 2.0);

                if (tanDist >= len1 || tanDist >= len2) return;

                // Äiá»ƒm tiáº¿p xÃºc (tangent points) â€” nÆ¡i arc cháº¡m vÃ o line
                Point3d tp1 = baseP + n1 * tanDist;
                Point3d tp2 = baseP + n2 * tanDist;

                // TÃ¢m arc
                var bisector = (n1 + n2).GetNormal();
                Point3d center = baseP + bisector * centerDist;

                // GÃ³c arc
                double a1 = Math.Atan2(tp1.Y - center.Y, tp1.X - center.X);
                double a2 = Math.Atan2(tp2.Y - center.Y, tp2.X - center.X);

                // Chá»n chiá»u arc ngáº¯n nháº¥t
                double sweep = a2 - a1;
                while (sweep > Math.PI) sweep -= 2 * Math.PI;
                while (sweep < -Math.PI) sweep += 2 * Math.PI;

                double arcStart = sweep >= 0 ? a1 : a2;
                double arcEnd = sweep >= 0 ? a2 : a1;
                while (arcStart < 0) arcStart += 2 * Math.PI;
                while (arcEnd < arcStart) arcEnd += 2 * Math.PI;

                // Váº½ line tá»« tangent point â†’ Ä‘áº§u cuá»‘i (Ä‘Ã£ trim)
                AddLine(btr, tr, tp1, slitEnd, false, COLOR_SLIT);
                AddLine(btr, tr, tp2, leadEnd, false, COLOR_SLIT);

                // Váº½ arc fillet
                var arc = new Arc(center, filletR, arcStart, arcEnd)
                {
                    Layer = LAYER,
                    Linetype = "BYLAYER",
                    Color = AcColor.FromColorIndex(ColorMethod.ByAci, COLOR_SLIT)
                };
                btr.AppendEntity(arc);
                tr.AddNewlyCreatedDBObject(arc, true);
            }
            catch { }
        }

        /// â”€â”€ MAIN DRAW â”€â”€
        public static void Draw(BoxParams p, BlockTableRecord btr, Transaction tr)
        {
            double L = p.Length, W = p.Width, T = p.Thickness, N = p.Nobi;
            double H1 = p.H_Bottom, H2 = p.H_Right, H3 = p.H_Top, H4 = p.H_Left;
            double sl = p.SlitLen, lv = p.LeadVal;
            double bx = p.BasePoint.X, by = p.BasePoint.Y;

            Point3d Pt(double x, double y) => Geo.P(bx + x, by + y);

            // Corner points
            var P1 = Pt(0, 0);
            var P2 = Pt(L, 0);
            var P3 = Pt(L, W);
            var P4 = Pt(0, W);

            // NOBI inner
            var P1i = Pt(N, N);
            var P2i = Pt(L - N, N);
            var P3i = Pt(L - N, W - N);
            var P4i = Pt(N, W - N);

            // â”€â”€ Dashed base rectangle â”€â”€
            EnsureLinetype(tr.GetObject(
                Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
                    .Database.LinetypeTableId, OpenMode.ForRead) as LinetypeTable,
                tr, "DASHED");

            // The outer NOBI rectangle is interrupted wherever the laser
            // contour uses the same T-wide corner segment. This keeps the
            // dashed bend/reference geometry separate from the solid cut path.
            if (p.IsLongX)
            {
                AddLine(btr, tr, Pt(T, 0), Pt(L - T, 0), true);
                AddLine(btr, tr, P2, P3, true);
                AddLine(btr, tr, Pt(L - T, W), Pt(T, W), true);
                AddLine(btr, tr, P4, P1, true);
            }
            else
            {
                AddLine(btr, tr, P1, P2, true);
                AddLine(btr, tr, Pt(L, T), Pt(L, W - T), true);
                AddLine(btr, tr, P3, P4, true);
                AddLine(btr, tr, Pt(0, W - T), Pt(0, T), true);
            }

            // â”€â”€ NOBI offset â”€â”€
            if (2 * N < L && 2 * N < W)
            {
                AddLine(btr, tr, P1i, P2i, true);
                AddLine(btr, tr, P2i, P3i, true);
                AddLine(btr, tr, P3i, P4i, true);
                AddLine(btr, tr, P4i, P1i, true);
            }

            // Outer corner fillet radius
            double R_outer = (p.Material.ToUpper() == "SS" && p.Thickness >= 6.0 && p.Thickness <= 9.0) ? 2.0 : 0.5;

            if (p.IsLongX)
            {
                // â•â•â• CASE 1: Length >= Width â•â•â•
                // Bottom/Top lui T (3 nÃ©t, khÃ´ng Ä‘Ã¨ base), Right/Left full

                // -- Bottom Flap (lui T) â€” 3 nÃ©t + fillet corners --
                AddLine(btr, tr, Pt(T, 0), Pt(T, -(H1 - N) + R_outer));
                AddLine(btr, tr, Pt(T + R_outer, -(H1 - N)), Pt(L - T - R_outer, -(H1 - N)));
                AddLine(btr, tr, Pt(L - T, -(H1 - N) + R_outer), Pt(L - T, 0));

                // -- Top Flap (lui T) â€” 3 nÃ©t + fillet corners --
                AddLine(btr, tr, Pt(L - T, W), Pt(L - T, W + (H3 - N) - R_outer));
                AddLine(btr, tr, Pt(L - T - R_outer, W + (H3 - N)), Pt(T + R_outer, W + (H3 - N)));
                AddLine(btr, tr, Pt(T, W + (H3 - N) - R_outer), Pt(T, W));

                // -- Right Flap (full) â€” 3 nÃ©t + fillet corners --
                AddLine(btr, tr, Pt(L - T, 0), Pt(L + (H2 - N) - R_outer, 0));
                AddLine(btr, tr, Pt(L + (H2 - N), 0 + R_outer), Pt(L + (H2 - N), W - R_outer));
                AddLine(btr, tr, Pt(L + (H2 - N) - R_outer, W), Pt(L - T, W));

                // -- Left Flap (full) â€” 3 nÃ©t + fillet corners --
                AddLine(btr, tr, Pt(T, W), Pt(-(H4 - N) + R_outer, W));
                AddLine(btr, tr, Pt(-(H4 - N), W - R_outer), Pt(-(H4 - N), 0 + R_outer));
                AddLine(btr, tr, Pt(-(H4 - N) + R_outer, 0), Pt(T, 0));

                // -- Outer fillet arcs (8 corners) --
                // Bottom Flap bottom-left
                AddArc(btr, tr, Pt(T + R_outer, -(H1 - N) + R_outer), R_outer, 180, 270, 0);
                // Bottom Flap bottom-right
                AddArc(btr, tr, Pt(L - T - R_outer, -(H1 - N) + R_outer), R_outer, 270, 360, 0);
                // Right Flap bottom-right
                AddArc(btr, tr, Pt(L + (H2 - N) - R_outer, 0 + R_outer), R_outer, 270, 360, 0);
                // Right Flap top-right
                AddArc(btr, tr, Pt(L + (H2 - N) - R_outer, W - R_outer), R_outer, 0, 90, 0);
                // Top Flap top-right
                AddArc(btr, tr, Pt(L - T - R_outer, W + (H3 - N) - R_outer), R_outer, 0, 90, 0);
                // Top Flap top-left
                AddArc(btr, tr, Pt(T + R_outer, W + (H3 - N) - R_outer), R_outer, 90, 180, 0);
                // Left Flap top-left
                AddArc(btr, tr, Pt(-(H4 - N) + R_outer, W - R_outer), R_outer, 90, 180, 0);
                // Left Flap bottom-left
                AddArc(btr, tr, Pt(-(H4 - N) + R_outer, 0 + R_outer), R_outer, 180, 270, 0);

                // -- Slits táº¡i 4 gÃ³c lui T (Bottom/Top) --
                DrawSlit(btr, tr, Pt(T, 0), Pt(T, sl), Pt(T - lv, -lv));
                DrawSlit(btr, tr, Pt(L - T, 0), Pt(L - T, sl), Pt(L - T + lv, -lv));
                DrawSlit(btr, tr, Pt(L - T, W), Pt(L - T, W - sl), Pt(L - T + lv, W + lv));
                DrawSlit(btr, tr, Pt(T, W), Pt(T, W - sl), Pt(T - lv, W + lv));
            }
            else
            {
                // â•â•â• CASE 2: Width > Length â•â•â•
                // Bottom/Top full, Right/Left lui T (3 nÃ©t, khÃ´ng Ä‘Ã¨ base)

                // -- Bottom Flap (full) â€” 3 nÃ©t + fillet corners --
                AddLine(btr, tr, P1, Pt(0, -(H1 - N) + R_outer));
                AddLine(btr, tr, Pt(R_outer, -(H1 - N)), Pt(L - R_outer, -(H1 - N)));
                AddLine(btr, tr, Pt(L, -(H1 - N) + R_outer), P2);
                AddLine(btr, tr, P1, Pt(0, T));
                AddLine(btr, tr, P2, Pt(L, T));

                // -- Top Flap (full) â€” 3 nÃ©t + fillet corners --
                AddLine(btr, tr, P3, Pt(L, W + (H3 - N) - R_outer));
                AddLine(btr, tr, Pt(L - R_outer, W + (H3 - N)), Pt(R_outer, W + (H3 - N)));
                AddLine(btr, tr, Pt(0, W + (H3 - N) - R_outer), P4);
                AddLine(btr, tr, P3, Pt(L, W - T));
                AddLine(btr, tr, P4, Pt(0, W - T));

                // -- Right Flap (lui T) â€” 3 nÃ©t + fillet corners --
                AddLine(btr, tr, Pt(L, T), Pt(L + (H2 - N) - R_outer, T));
                AddLine(btr, tr, Pt(L + (H2 - N), T + R_outer), Pt(L + (H2 - N), W - T - R_outer));
                AddLine(btr, tr, Pt(L + (H2 - N) - R_outer, W - T), Pt(L, W - T));

                // -- Left Flap (lui T) â€” 3 nÃ©t + fillet corners --
                AddLine(btr, tr, Pt(0, W - T), Pt(-(H4 - N) + R_outer, W - T));
                AddLine(btr, tr, Pt(-(H4 - N), W - T - R_outer), Pt(-(H4 - N), T + R_outer));
                AddLine(btr, tr, Pt(-(H4 - N) + R_outer, T), Pt(0, T));

                // -- Outer fillet arcs (8 corners) --
                // Bottom Flap bottom-left
                AddArc(btr, tr, Pt(0 + R_outer, -(H1 - N) + R_outer), R_outer, 180, 270, 0);
                // Bottom Flap bottom-right
                AddArc(btr, tr, Pt(L - R_outer, -(H1 - N) + R_outer), R_outer, 270, 360, 0);
                // Right Flap bottom-right
                AddArc(btr, tr, Pt(L + (H2 - N) - R_outer, T + R_outer), R_outer, 270, 360, 0);
                // Right Flap top-right
                AddArc(btr, tr, Pt(L + (H2 - N) - R_outer, W - T - R_outer), R_outer, 0, 90, 0);
                // Top Flap top-right
                AddArc(btr, tr, Pt(L - R_outer, W + (H3 - N) - R_outer), R_outer, 0, 90, 0);
                // Top Flap top-left
                AddArc(btr, tr, Pt(0 + R_outer, W + (H3 - N) - R_outer), R_outer, 90, 180, 0);
                // Left Flap top-left
                AddArc(btr, tr, Pt(-(H4 - N) + R_outer, W - T - R_outer), R_outer, 90, 180, 0);
                // Left Flap bottom-left
                AddArc(btr, tr, Pt(-(H4 - N) + R_outer, T + R_outer), R_outer, 180, 270, 0);

                // -- Slits táº¡i 4 gÃ³c lui T (Right/Left) --
                DrawSlit(btr, tr, Pt(L, T), Pt(L - sl, T), Pt(L + lv, T - lv));
                DrawSlit(btr, tr, Pt(L, W - T), Pt(L - sl, W - T), Pt(L + lv, W - T + lv));
                DrawSlit(btr, tr, Pt(0, W - T), Pt(sl, W - T), Pt(-lv, W - T + lv));
                DrawSlit(btr, tr, Pt(0, T), Pt(sl, T), Pt(-lv, T - lv));
            }
        }

        static void EnsureLinetype(LinetypeTable ltt, Transaction tr, string name)
        {
            if (ltt == null || ltt.Has(name)) return;
            var db = Autodesk.AutoCAD.ApplicationServices.Core.Application
                        .DocumentManager.MdiActiveDocument.Database;
            try { db.LoadLineTypeFile(name, "acad.lin"); } catch { }
        }
    }

    // â”€â”€â”€ AUTOCAD COMMAND â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    public class BoxMetCommand
    {
        [CommandMethod("BOXMET", CommandFlags.Modal)]
        public void RunBoxMet()
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application
                        .DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;

            ed.WriteMessage("\nâ•”â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•—");
            ed.WriteMessage("\nâ•‘   BOXMET â€” Sheet Metal Flat Pattern   â•‘");
            ed.WriteMessage("\nâ•šâ•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•â•\n");

            var dlg = new BoxMetDialog();
            var result = dlg.ShowDialog();
            if (result != System.Windows.Forms.DialogResult.OK)
            {
                ed.WriteMessage("\n>> BOXMET: ÄÃ£ há»§y.\n");
                return;
            }

            var p = dlg.GetParams();
            var ptOpt = new PromptPointOptions("\n>> Chá»n Ä‘iá»ƒm Ä‘áº·t hÃ¬nh (Base Point): ");
            var ptRes = ed.GetPoint(ptOpt);
            if (ptRes.Status != PromptStatus.OK) return;
            p.BasePoint = ptRes.Value;

            using (var tr = db.TransactionManager.StartTransaction())
            {
                var bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                var btr = (BlockTableRecord)tr.GetObject(
                            bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                DrawEngine.Draw(p, btr, tr);
                tr.Commit();
            }

            double R_outer = (p.Material.ToUpper() == "SS" && p.Thickness >= 6.0 && p.Thickness <= 9.0) ? 2.0 : 0.5;
            ed.WriteMessage($"\n>> BOXMET: HoÃ n thÃ nh! L={p.Length} W={p.Width} T={p.Thickness}");
            ed.WriteMessage($"\n>> Mode: {(p.IsLongX ? "Lâ‰¥W (Bottom/Top lui T)" : "W>L (Left/Right lui T)")}");
            ed.WriteMessage($"\n>> Layer 0, slit mÃ u 3, outer corner R={R_outer}\n");
            doc.SendStringToExecute("_.ZOOM _E ", true, false, false);
        }
    }
}


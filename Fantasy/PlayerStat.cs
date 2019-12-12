using DataTransferLibrary.Models.PlayerSeasonAverages;

namespace Fantasy
{
    public class PlayerStat : Datum
    {

        public double? WeeklyIndex;

        public PlayerStat(int player_id, double? pts, double? blk, double? stl, double? ast, double? reb, double? turnover, double? fg_pct, double? fg3_pct, double? ft_pct)
        {
            this.player_id = player_id;
            this.pts = pts;
            this.blk = blk;
            this.stl = stl;
            this.ast = ast;
            this.reb = reb;
            this.turnover = turnover;
            this.fg_pct = fg_pct;
            this.fg3_pct = fg3_pct;
            this.ft_pct = ft_pct;
            this.WeeklyIndex = CalculatePlayerIndex();
        }

        private double? CalculatePlayerIndex()
        {
            return pts + blk * 2 + stl * 2 + ast * 2 + reb - (turnover * 2) + pts * (1+ (fg_pct/100 + fg3_pct/100 + ft_pct/100));
        }

    }
}

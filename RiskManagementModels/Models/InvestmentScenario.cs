namespace RiskManagementModels.Models
{
    public class InvestmentScenario
    {
        /// <summary>
        /// Вероятность сценария
        /// </summary>
        public double Probability { get; set; }
        /// <summary>
        /// Объем выпуска, единиц
        /// </summary>
        public double Q { get; set; }
        /// <summary>
        /// Цена за единицу, руб.
        /// </summary>
        public double P { get; set; }
        /// <summary>
        /// Переменные затраты за единицу, руб.
        /// </summary>
        public double V { get; set; }
        /// <summary>
        /// Постоянные затраты, руб.
        /// </summary>
        public double F { get; set; }
        /// <summary>
        /// Амортизационные начисления, руб.
        /// </summary>
        public double A { get; set; }
        /// <summary>
        /// Налог на прибыль, %
        /// </summary>
        public double N { get; set; }
        /// <summary>
        /// Норма дисконта,%
        /// </summary>
        public double r { get; set; }
        /// <summary>
        /// Срок проекта, лет
        /// </summary>
        public double T { get; set; }
        /// <summary>
        /// Начальные инвестиции, руб.
        /// </summary>
        public double I { get; set; }

        public InvestmentScenario(double probability)
        {
            Probability = probability;
        }

        public double GetNPV(int years, double fModify, double qModify = 0, double pModify = 0)
        {
            var summaryByYears = 0.0;
            for(int i = 1; i < years; i++)
            {
                summaryByYears += GetRt(i, fModify, qModify, pModify) / Math.Pow((1 + r), i);
            }

            return summaryByYears - I;
        }

        public double GetNPVConsideringInflation(double averageMonthInflation, double d, int years, double fModify)
        {
            var summaryByYears = 0.0;
            for (int i = 1; i < years; i++)
            {
                summaryByYears += GetRt(i, fModify) / Math.Pow(GetRInflation(averageMonthInflation, d), i);
            }

            return summaryByYears - I;
        }

        public double GetRt(int currentYear, double fModify, double qModify = 0, double pModify = 0)
        {
            return ((Q + qModify) * ((P + pModify) - V) - F * Math.Pow(1 + fModify, currentYear)  - A) * (1 - N) + A;
        }

        public double GetProfitabilityIndex(int years, double fModify)
        {
            var summaryByYears = 0.0;
            for (int i = 1; i < years; i++)
            {
                summaryByYears += GetRt(i, fModify) / Math.Pow((1 + r), i);
            }

            return summaryByYears / I;
        }

        public int GetPaybackPeriod(double fModify)
        {
            int i = 1;
            var summaryByYears = GetRt(i, fModify) / Math.Pow((1 + r), i);
            while (summaryByYears < 0)
            {
                i++;
                summaryByYears += GetRt(i, fModify) / Math.Pow((1 + r), i);
            }

            return i;
        }

        private double GetRInflation(double averageMonthInflation, double d)
        {
            return Math.Pow((1 + averageMonthInflation), 12) * (1 + d);
        }
    }
}

using System;
using MathNet;
using MathNet.Numerics.Distributions;

namespace RiskManagementModels.Models
{
    public class InvestmentProject
    {
        public InvestmentProject(string projectName)
        {
            ProjectName = projectName;
        }

        public string ProjectName { get; set; }
        /// <summary>
        /// Список возможных сценариев 
        /// </summary>
        public List<InvestmentScenario> Scenarios { get; set; }

        public void GetScenariosNPV(int years, double fModify)
        {
            for(int i = 0; i < Scenarios.Count; i++)
            {
                var NPVi = Scenarios[i].GetNPV(years, fModify);
                Console.WriteLine($" Для проекта {ProjectName}: ");
                Console.WriteLine($" При T = {years}, | NPVi = {NPVi} для сценария номер {i + 1}");
                Console.WriteLine();
            }
            SetIndentation();
        }

        public void GetScenariosProfitabilityIndex(int years, double fModify)
        {
            for(int i = 0; i < Scenarios.Count; i++)
            {
                var prIndex = Scenarios[i].GetProfitabilityIndex(years, fModify);
                Console.WriteLine($" Для проекта {ProjectName}: ");
                Console.WriteLine($" При T = {years}, | Индекс окупаемости: {prIndex}");
                Console.WriteLine();
            }
            SetIndentation();
        }

        public void GetPaybackPeriodForLastScenario(double fModify)
        {
            var payBackPeriod = Scenarios.Last().GetPaybackPeriod(fModify);
            Console.WriteLine($" Для проекта {ProjectName}: ");
            Console.WriteLine($" Срок окупаемости последнего сценария: {payBackPeriod}");
            SetIndentation();
        }

        public double GetAverageNPV(int years, double fModify, bool hidden = false)
        {
            var averageNPV = 0.0;
            for (int i = 0; i < Scenarios.Count; i++)
                averageNPV += Scenarios[i].GetNPV(years, fModify) * Scenarios[i].Probability;

            if (!hidden)
            {
                Console.WriteLine($" Для проекта {ProjectName}: ");
                Console.WriteLine($" При T = {years}, | Средняя чистая приведенная стоимость: {averageNPV}");
                SetIndentation();
            }

            return averageNPV;
        }

        private void SetIndentation(int count = 3)
        {
            for (int i = 0; i < count; i++)
                Console.WriteLine();
        }

        public (double, double) GetDataSensitivity(int years, double fModify, double qModify, double pModify)
        {
            var qSensitivity = Math.Abs(GetSensitity(years, fModify, qModify, 0));
            var pSensitivity = Math.Abs(GetSensitity(years, fModify, 0, pModify));

            Console.WriteLine($" Для проекта {ProjectName}: ");
            Console.WriteLine($" При изменении Q на {qModify * 100}%, | Чувствительность данных: {qSensitivity}%");
            Console.WriteLine($" При изменении P на {pModify * 100}%, | Чувствительность данных: {pSensitivity}%");

            SetIndentation();

            return (qSensitivity, pSensitivity);
        }

        private double GetSensitity(int years, double fModify, double qModify, double pModify)
        {
            var NPV = Scenarios.Last().GetNPV(years, fModify);
            var NPVmodify = Scenarios.Last().GetNPV(years, fModify, qModify, pModify);

            return ((NPVmodify - NPV) / NPV) * 100;
        }

        public double GetDespersion(int years, double fModify, bool hidden = false)
        {
            var despersion = 0.0;
            for(int i = 0; i < Scenarios.Count; i++)
            {
                despersion += Math.Pow(Scenarios[i].GetNPV(years, fModify), 2) * Scenarios[i].Probability;
            }
            despersion -= Math.Pow(GetAverageNPV(years, fModify, hidden: true), 2);

            if (!hidden)
            {
                Console.WriteLine($" Для проекта {ProjectName}: ");
                Console.WriteLine($" При T = {years}, | Дисперсия: {despersion}");
                SetIndentation();
            }

            return despersion;
        }

        public double GetProbability(int years, double fModify, double targetNPV)
        {
            var probabilities = Scenarios.Select(scenario => scenario.Probability).ToList();
            var NPVs = Scenarios.Select(scenario => scenario.GetNPV(years, fModify)).ToList();
            // Шаг 1: Расчет средней NPV
            double npvAvg = 0;
            for (int i = 0; i < NPVs.Count; i++)
            {
                npvAvg += NPVs[i] * probabilities[i];
            }

            var variance = GetDespersion(years, fModify, true);

            // Шаг 3: Расчет Z-значения
            double z = (targetNPV - npvAvg) / Math.Sqrt(variance);

            // Шаг 4: Расчет вероятности P(NPV < 30000) с использованием стандартного нормального распределения
            double probability = Normal.CDF(0, 1, z);

            Console.WriteLine($" Для проекта {ProjectName}: ");
            Console.WriteLine($" При T = {years}, | Вероятность, что доходность будет меньше {targetNPV} составляет: {probability}");
            SetIndentation();

            return probability;
        }

        public void GetNPVConsideringInflation(double averageMonthInflation, double d, int years, double fModify, bool hidden = false)
        {
            for (int i = 0; i < Scenarios.Count; i++)
            {
                var NPVi = Scenarios[i].GetNPVConsideringInflation(averageMonthInflation, d, years, fModify);
                Console.WriteLine($" Для проекта {ProjectName}: ");
                Console.WriteLine($" При среднемесячной инфляции {averageMonthInflation} и требуемой ставке доходности {d}");
                Console.WriteLine($" При T = {years}, | Средняя чистая приведенная стоимость с учетом инфляции: = {NPVi} для сценария номер {i + 1}");
                Console.WriteLine();
            }
        }

        public double GetRandomNPV(double z, int years, double fModify)
        {
            //var normalDistribution = new MathNet.Numerics.Distributions.Normal();
            //var value = normalDistribution.CumulativeDistribution(z);
            return Scenarios[2].GetNPV(years, fModify);
        }
    }
}

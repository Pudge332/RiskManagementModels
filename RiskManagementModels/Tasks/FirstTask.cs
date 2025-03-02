using RiskManagementModels.Models;

namespace RiskManagementModels.Tasks
{
    public class FirstTask
    {
        private InvestmentProject X;
        private InvestmentProject Y;
        private int M = 10;

        public FirstTask()
        {
            X = new InvestmentProject("X")
            {
                Scenarios = new List<InvestmentScenario>()
                {
                    new InvestmentScenario(0.2)
                    {
                        Q = 15000,
                        P = 1500,
                        V = 1400,
                        F = 2500,
                        A = 80,
                        N = 20,
                        r = 12,
                        T = 5,
                        I = 30000
                    },
                    new InvestmentScenario(0.3)
                    {
                        Q = 25000,
                        P = 2500,
                        V = 1000,
                        F = 2500,
                        A = 200,
                        N = 20,
                        r = 8,
                        T = 5,
                        I = 30000
                    },
                    new InvestmentScenario(0.5)
                    {
                        Q = 20000,
                        P = 2000,
                        V = 1200,
                        F = 2500,
                        A = 100,
                        N = 20,
                        r = 10,
                        T = 5,
                        I = 30000
                    }
                }
            };

            Y = new InvestmentProject("Y")
            {
                Scenarios = new List<InvestmentScenario>()
                {
                    new InvestmentScenario(0.2)
                    {
                        Q = 5000,
                        P = 23500,
                        V = 17000,
                        F = 20000,
                        A = 80,
                        N = 0.2,
                        r = 15,
                        T = 5,
                        I = 56000000 / M
                    },
                    new InvestmentScenario(0.3)
                    {
                        Q = 7000,
                        P = 27500,
                        V = 14000,
                        F = 20000,
                        A = 200,
                        N = 0.2,
                        r = 8,
                        T = 5,
                        I = 56000000 / M
                    },
                    new InvestmentScenario(0.5)
                    {
                        Q = 6000,
                        P = 25000,
                        V = 15200,
                        F = 20000,
                        A = 100,
                        N = 0.2,
                        r = 10,
                        T = 5,
                        I = 56000000 / M
                    },
                }
            };
        }

        public void RunTask()
        {
            var investmentProjects = new List<InvestmentProject>() { X, Y };
            
            int years = 15;
            double fModify = 0.05;
            double dataSensativity = 0.01;
            double averageMonthInflation = 1.2;
            double z = 30000;

            investmentProjects.ForEach(project =>
            {
                project.GetScenariosNPV(years, fModify);
                project.GetScenariosProfitabilityIndex(years, fModify);
                project.GetPaybackPeriodForLastScenario(fModify);
                project.GetDataSensitivity(years, fModify, dataSensativity, dataSensativity);
                project.GetAverageNPV(years, fModify);
                project.GetDespersion(years, fModify);
                project.GetProbability(years, fModify, z);
                project.GetNPVConsideringInflation(averageMonthInflation, dataSensativity, years, fModify);
            });
        }
    }
}

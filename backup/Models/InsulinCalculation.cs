namespace LiveDiabetes.Models
{
    public class InsulinCalculation
    {
        public double CalculateInsulinDose(double currentValue, double idealValue, double fsi, double carbs, double ratio)
        {
            if (fsi == 0 || ratio == 0)
            {
                throw new ArgumentException("FSI e Ratio devem ser diferentes de zero.");
            }

            double y = (currentValue - idealValue) / fsi;
            double z = carbs / ratio;
            double result = y + z;

            // Arredondar o resultado para o inteiro mais próximo
            return Math.Round(result);
        }
    }
}
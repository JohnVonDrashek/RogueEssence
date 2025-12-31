using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueEssence.LevelGen
{
    /// <summary>
    /// Divides a given amount of money into a specified number of pickups.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class MoneyDivSpawner<T> : IStepSpawner<T, MoneySpawn>, IDivSpawner
        where T : ISpawningGenContext<MoneySpawn>
    {
        //amounts cannot be over this % greater or less than the base value
        const int DIV_DIFF = 50;

        /// <summary>
        /// The number of pickups to split the total sum of money into.
        /// </summary>
        public RandRange DivAmount { get; set; }

        /// <summary>
        /// Initializes a new instance of the MoneyDivSpawner class.
        /// </summary>
        public MoneyDivSpawner() { }

        /// <summary>
        /// Initializes a new instance of the MoneyDivSpawner class with the specified division amount range.
        /// </summary>
        /// <param name="divAmount">The range for number of pickups to create.</param>
        public MoneyDivSpawner(RandRange divAmount)
        {
            DivAmount = divAmount;
        }

        /// <summary>
        /// Gets the money spawns by dividing the total amount into multiple pickups.
        /// </summary>
        /// <param name="map">The generation context with spawner data.</param>
        /// <returns>A list of money spawns divided from the total.</returns>
        public List<MoneySpawn> GetSpawns(T map)
        {
            MoneySpawn total = map.Spawner.Pick(map.Rand);
            int chosenDiv = Math.Min(total.Amount, Math.Max(1, DivAmount.Pick(map.Rand)));
            int avgAmount = total.Amount / chosenDiv;
            int currentTotal = 0;
            List<MoneySpawn> results = new List<MoneySpawn>();
            for (int ii = 0; ii < chosenDiv; ii++)
            {
                int nextTotal = total.Amount;
                if (ii + 1 < chosenDiv)
                {
                    int expectedCurrentTotal = total.Amount * (ii+1) / chosenDiv;
                    int amount = avgAmount * (/*map.Rand.Next(DIV_DIFF * 2)*/((ii % 2 == 0) ? 0 : 99) - DIV_DIFF) / 200;
                    nextTotal = expectedCurrentTotal + amount;
                }
                if (nextTotal > currentTotal)
                    results.Add(new MoneySpawn(nextTotal - currentTotal));
                currentTotal = nextTotal;
            }
            return results;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", this.GetType().GetFormattedTypeName(), this.DivAmount.ToString());
        }
    }

    /// <summary>
    /// Interface for spawners that divide a total amount into multiple spawns.
    /// </summary>
    public interface IDivSpawner
    {
        /// <summary>
        /// Gets or sets the random range for the number of divisions.
        /// </summary>
        RandRange DivAmount { get; set; }
    }
}

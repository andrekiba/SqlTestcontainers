using System.Reflection;
using Xunit.Sdk;

namespace SqlTestcontainers.IntegrationTests3
{
    /**
     * Apply this to a Theory in order to run it multiple times.
     * The test method needs to take a single int parameter,
     * representing the iteration counter, but it doesn't need to
     * use it.
     *
     * Usage:
     *   [Theory, Repeat(5)]
     *   public void MyTest(int i)
     *   {
     *   }
     */
    public class RepeatAttribute : DataAttribute
    {
        readonly int count;

        public RepeatAttribute(int count)
        {
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(count),
                    "Repeat count must be greater than 0.");
            }

            this.count = count;
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            return Enumerable.Range(1, count).Select(BuildParamArray);
        }

        protected virtual object[] BuildParamArray(int i)
        {
            return [i];
        }
    }

    /**
     * Apply this to a Theory in order to run it multiple times
     * with provided data. The test method's first parameter
     * must be an int, representing the iteration counter.
     *
     * Usage:
     *   [Theory, Repeat(5, "foo")]
     *   public void MyTest(int i, string tag)
     *   {
     *   }
     */
    public class RepeatDataAttribute(int count, params object[] data) : RepeatAttribute(count)
    {
        readonly List<object> data = data.ToList();

        protected override object[] BuildParamArray(int i)
        {
            return data.Prepend(i).ToArray();
        }
    }
}
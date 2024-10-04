using Quantum.Inspector;

namespace Quantum
{

    public class NamedElementAttribute : PropertyAttribute
    {
        public readonly string VariableName;
        public NamedElementAttribute(string variableName) { this.VariableName = variableName; }
    }
}
using System;

namespace MHGameWork.TheWizards.GodGame.Persistence.POSystem
{
    /// <summary>
    /// Value can either be a primitive type, a struct(complex value) or a reference (so some sort of C++ union)
    /// Note that only references require that a type is stored 
    ///     (since they are the only polymorphic type, and for the others the type can be determined by the deserialization target)
    ///     but for now, for simplicity, the type is always stored
    /// </summary>
    public struct SerializedValue
    {
        public SerializedType Type;

        // One of the three
        public string SimpleValue;
        public SerializedAttribute[] ComplexAttributes;
        public SerializedValue[] ArrayElements;
        public POIdentifier ReferenceIdentifier;

        public static SerializedValue NullReference
        {
            get { return new SerializedValue() { Type = null }; }
        }
        public bool IsNullReference() { return Type == null; }


        public static SerializedValue CreateArray(SerializedType serializedType, SerializedValue[] elements)
        { return new SerializedValue { Type = serializedType, ArrayElements = elements }; }
        public static SerializedValue CreateSimple(SerializedType serializedType, string serializedPrimitive)
        { return new SerializedValue { Type = serializedType, SimpleValue = serializedPrimitive }; }
        public static SerializedValue CreateComplex(SerializedType serializedType, SerializedAttribute[] attributes)
        { return new SerializedValue { Type = serializedType, ComplexAttributes = attributes }; }
        public static SerializedValue CreateReference(SerializedType serializedType, POIdentifier reference)
        { return new SerializedValue { Type = serializedType, ReferenceIdentifier = reference }; }


        public bool IsSimple() { return SimpleValue != null; }
        public bool IsComplex() { return ComplexAttributes != null; }
        public bool IsArray() { return ArrayElements != null; }
        public bool IsReference() { return ReferenceIdentifier != null; }

        public override string ToString()
        {
            if (IsNullReference()) return "NULL";
            if (SimpleValue != null) return String.Format("SIMPLE Type: {0}, SimpleValue: {1}", Type, SimpleValue);
            if (ComplexAttributes != null) return String.Format("COMPLEX Type: {0},  ComplexAttributes: {1}}", Type, ComplexAttributes);
            if (ArrayElements != null) return String.Format("ARRAY Type: {0},  ArrayElements: {1}", Type, ArrayElements);
            if (ReferenceIdentifier != null) return String.Format("REFERENCE Type: {0}, ReferenceIdentifier: {1}", Type, ReferenceIdentifier);

            return String.Format("INVALID!!!! Type: {0}, SimpleValue: {1}, ComplexAttributes: {2}, ArrayElements: {3}, ReferenceIdentifier: {4}", Type, SimpleValue, ComplexAttributes, ArrayElements, ReferenceIdentifier);
        }
    }
}
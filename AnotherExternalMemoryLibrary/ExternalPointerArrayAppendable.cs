namespace AnotherExternalMemoryLibrary
{
    public class ExternalPointerArrayAppendable<T> : ExternalPointerArray<T> where T : unmanaged
    {
        public ExternalPointerArrayAppendable(IntPtrEx Handle, int Length) : base(Handle, Length)
        {
        }

        public ExternalPointerArrayAppendable(IntPtrEx Handle, params T[] Values) : base(Handle, Values)
        {
        }

        public ExternalPointerArrayAppendable(IntPtrEx Handle, int Length, IntPtrEx Address) : base(Handle, Length, Address)
        {
        }

        public int Index { get; set; } = 0;

        public void Append(T value)
        {
            if (Index >= Length)
            {
                throw new System.IndexOutOfRangeException();
            }
            else
            {
                this[Index] = value;
                Index++;
            }
        }

        public void AppendRange(T[] values)
        {
            if (Index + values.Length >= Length)
            {
                throw new System.IndexOutOfRangeException();
            }
            else
            {
                WriteRange(Index, values);
                Index += values.Length;
            }
        }
    }
}

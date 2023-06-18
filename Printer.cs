using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace NetcodeIO.NET
{
    public struct Printer
    {
        public Printer(params Type[] types)
        {
            foreach (var type in types)
            {
                if (type == GetType()) continue;
                if (type.IsSubclassOf(typeof(Attribute))) continue;
                if (type.IsAbstract && type.IsSealed) continue;
                
                Print(type, string.Empty, string.Empty);
                PrintContentRecursive(type, 1);

                Console.WriteLine();
            }
        }

        public Printer() : this(typeof(Printer).Assembly.GetTypes().ToArray()) { }
        
        private void PrintContentRecursive(Type type, int indent)
        {
            var indentStr = $"{Enumerable.Repeat(" ", Math.Clamp(indent - 1, 0, int.MaxValue))}+-";
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                type = field.FieldType;
                if (type == GetType()) continue;

                Print(type, indentStr, $"{field.Name}: ");
            }
        }

        private static void Print(Type type, string indent = "", string prefix = "")
        {
            var size = SizeOf(type, out var sizeofEx);
            var marshal = MarshalSize(type, out var marshalEx);
            
            Console.WriteLine($"{indent}{prefix}{type.FullName}: {Format(size)}({Format(marshal)} marshal)");
            var c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            if (sizeofEx != null) Console.WriteLine($"{indent} :{sizeofEx.Message}");
            if (marshalEx != null) Console.WriteLine($"{indent} :{marshalEx.Message}");
            Console.ForegroundColor = c;
        }

        private static string Format(int? size)
        {
            return (size.HasValue ? size.Value.ToString() : "(?)") + "b";
        }
        
        private static int? SizeOf(Type type, out Exception exception)
        {
            try
            {
                // all this just to invoke one opcode with no arguments!
                var method = new DynamicMethod("GetManagedSizeImpl", typeof(uint), Type.EmptyTypes, typeof(TypeExtensions), false);

                var gen = method.GetILGenerator();
                gen.Emit(OpCodes.Sizeof, type);
                gen.Emit(OpCodes.Ret);

                var func = (Func<uint>)method.CreateDelegate(typeof(Func<uint>));
                exception = null;
                return checked((int)func());
            }
            catch (Exception e)
            {
                exception = e;
                return null;
            }
        }

        private static int? MarshalSize(Type type, out Exception exception)
        {
            if (type.IsEnum) type = type.GetEnumUnderlyingType();
            
            try
            {
                exception = null;
                return Marshal.SizeOf(FormatterServices.GetUninitializedObject(type));
            }
            catch (Exception e)
            {
                exception = e;
                return null;
            }
        }

    }
}
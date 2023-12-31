using System;
using System.Text;

public class FInt32
{
    //members
    public Int32 m_raw { get; private set; } //raw underlying int32 value 
    //consts
    //Point = number of bits after decimal point in fixed representation, there will always be 32 bits total 
    public const byte POINT = 8; //default 8 = 24 bit signed integer, 8 bit unsigned fractional
    public static readonly FInt32 MAX = new FInt32(Int32.MaxValue); //max representable value
    public static readonly FInt32 MIN = new FInt32(Int32.MinValue); //min representable value
    public static readonly FInt32 ZERO = new FInt32(0); //Zero const
    public static readonly FInt32 ONE = new FInt32(1 << POINT); //1 shifted to smallest integral unit 
    public static readonly FInt32 HALF = new FInt32(1 << POINT - 1); //1 shifted to smallest fractional unit
    public static readonly FInt32 MIN_FRACTIONAL = new FInt32(1); //aka precision of data, we dont really need a MIN_INTEGRAL because we have... ONE...
    //max/masks
    public const Int32 MAX_FRACTIONAL = (1 << POINT) - 1; //max representable fraction value, also used as a mask to return only fractional info
    public const Int32 MAX_INTEGRAL = ~MAX_FRACTIONAL; //max representable integral value, also used as mask to only show integral values


    //ctor
    private FInt32(Int32 raw) => m_raw = raw; //private ctor

    //parts
    public Int32 Integral => m_raw & MAX_INTEGRAL; //mask out fractional 
    public Int32 Fractional => m_raw & MAX_FRACTIONAL; //mask out integral, could also write ~MAX_INTEGRAL but this is more readable imo

    //math functions
    public static int Sign(FInt32 a) => a == 0 ? 0 : a > 0 ? 1 : -1; //return sign of value (-1,1,0)
    public static FInt32 Floor(FInt32 a) => new FInt32(a.Integral); //remove fractional values (round down)
    public static FInt32 Ceiling(FInt32 a) => a.Fractional == 0 ? a : Floor(a + ONE); //if no fractional, return what we were passed, else return that plus 1 (round up)
    public static FInt32 Round(FInt32 a) => Floor(a + HALF); //round to nearest whole integer (could be up or down, 0.5 rounds up here)
    public static FInt32 Abs(FInt32 a) => a > ZERO ? a : -a; //return positive value of what we were passed
    public static FInt32 Clamp(FInt32 a, FInt32 min, FInt32 max) => a < min ? min : a > max ? max : a; //clamp between min and max

    public static FInt32 Pow(FInt32 a, int p)
    {
        if (p == 0) return ONE; //early out
        if (p <= 1) return a; //we only work with positive int exponents 
        FInt32 result = a;
        for (int i = 1; i < p; ++i) result *= a; //long buffer used in * operator to help against overflow, not needed explicitly here
        return result;
    }

    public static FInt32 Sqrt(FInt32 a)
    {
        //shamelessly translated from https://en.wikipedia.org/wiki/Integer_square_root#Example_implementation_in_C
        //get sqrt of raw value and return new FINT32 using that value
        if (a <= ZERO) return ZERO;
        if (a == ONE) return ONE;

        uint raw = (uint)a.m_raw;
        uint temp = raw; //use temp as while loop will change value
        byte bitLength = 0;
        while ((temp >>= 1) > 0) ++bitLength; //number of bits needed to store raw
        //wikipedia says this is the best guess for least number of iterations on average
        uint root = (uint)2 << ((bitLength >> 1) + 1); //initial guess

        uint update = (root + raw / root) >> 1; //first update
        while (update < root) //wont pass if root = update (meaning we arent changing anything)
        {
            root = update;
            update = (root + raw / root) >> 1; //update
        }
        return new FInt32((int)root << (POINT >> 1)); //actually not sure why we need the shifts here
        //it didnt work before but does now. lol.
    }

    //conversion
    public int ToInt => m_raw >> POINT; //returns truncated int, use Round for closest int value
    public float ToFloat => m_raw / (float)ONE.m_raw; //returns closest equivalent float (useful for when we want to give values to Unity after we simulate)
    public static FInt32 FromInt(int i) => new FInt32(i << POINT); //creates equivalent FINT32 from int
    public static FInt32 FromFloat(float f) => new FInt32((int)(ONE.m_raw * f)); //creates closest equivalent FINT32 from float  -- not sure if this is useful as the whole point of this class is to not be using floats, string ctor preferred
    public static FInt32 MakeFromRaw(Int32 raw) => new FInt32(raw); //public accessor to private ctor, creates from RAW INT32 (will not be equivalent to int value if the int != 0;


    //creates FINT32 from given ints, can be confusing when trying to make decimals with this as fractional representation is MIN_FRACTIONAL * passed fractional value, meaning passing in 0,50 will give a larger value than 0,5. prefer string ctor in most cases.
    public static FInt32 MakeFromParts(int integral, int fractional = 0) => new FInt32((integral << POINT) | fractional);
    //slowest but most human readable way of creating FINT32s, passing in "0.5" will give (closest possible representation of) the expected value, unlike MakeFromParts which will return  MIN_FRACTIONAL * 5
    public static FInt32 FromString(string s)
    {
        s = s.Trim();
        if (!double.TryParse(s, out double d)) return ZERO; //out if string is invalid;
        if (!s.Contains(".")) return new FInt32((int)d << POINT); //early return if no decimal
        string[] parts = s.Split('.');
        int.TryParse(parts[0], out int i); //integral
        bool zeroNegative = s[0] == '-' && i == 0; //special handling for numbers where -1 < value < 0
        i <<= POINT; //shift to correct integral place for FINT32
        int.TryParse(parts[1], out int f); //fractional
        int denominator = 10; //start at 10 to skip 1 guaranteed iteration
        for (int j = 1; j < parts[1].Length; ++j) { denominator *= 10; }; //get denominator (number of units * 10)
        int o = ONE.m_raw; //FINT32 one as raw int
        f *= o; //convert f to raw
        f /= denominator;//convert fraction from f to 0.f (as raw)
        if (i < 0 || zeroNegative) //handle value between -1 and 0
        {
            i -= o; //decrement integral
            f = o - f; //invert + incrememnt fractional
        }
        return new FInt32(i | (f & MAX_FRACTIONAL)); //return parsed string as FINT32 
    }

    public static FInt32 Min(FInt32 a, FInt32 b) => a < b ? a : b;
    public static FInt32 Max(FInt32 a, FInt32 b) => a < b ? b : a;

    //operators, should be mostly self-explanatory as we are just performing the maths on the underlying raw int values
    public static FInt32 operator +(FInt32 a, FInt32 b) => new FInt32(a.m_raw + b.m_raw);
    public static FInt32 operator -(FInt32 a, FInt32 b) => new FInt32(a.m_raw - b.m_raw);
    public static FInt32 operator -(FInt32 a) => ZERO - a;
    public static FInt32 operator %(FInt32 a, FInt32 b) => new FInt32(a.m_raw % b.m_raw);
    public static FInt32 operator *(FInt32 f, int i) => new FInt32(f.m_raw * i);
    public static FInt32 operator *(int i, FInt32 f) => new FInt32(f.m_raw * i); //have to explicity state we are order-agnostic for multiplication
    public static FInt32 operator *(FInt32 a, FInt32 b)
    {
        long buffer = (long)a.m_raw * b.m_raw;
        return new FInt32((int)buffer >> POINT);
        //if i write this in one line compiler tries to optimize out the long cast and we get overflow
    }

    public static FInt32 operator /(FInt32 a, FInt32 b)
    {
        long buffer = (long)a.m_raw << POINT;
        return new FInt32((int)(buffer / b.m_raw));
        //As above
    }
    public static FInt32 operator /(FInt32 f, int i) => new FInt32(f.m_raw / i);
    public static FInt32 operator /(int i, FInt32 f) => new FInt32(i / f.m_raw);
    public static bool operator <(FInt32 a, FInt32 b) => a.m_raw < b.m_raw;
    public static bool operator >(FInt32 a, FInt32 b) => a.m_raw > b.m_raw;
    public static bool operator >=(FInt32 a, FInt32 b) => a.m_raw >= b.m_raw;
    public static bool operator <=(FInt32 a, FInt32 b) => a.m_raw <= b.m_raw;
    public static bool operator ==(FInt32 a, FInt32 b) => a.m_raw == b.m_raw;
    public static bool operator !=(FInt32 a, FInt32 b) => a.m_raw != b.m_raw;

    //implicit int conversion, all integers that can be stores in (32 - POINT) bits are representable
    public static implicit operator FInt32(int i) => FromInt(i);

    //overrides to prevent warnings
    public override int GetHashCode() => m_raw.GetHashCode();
    public override bool Equals(object obj) => GetHashCode() == obj.GetHashCode();

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        int i = Integral;
        int f = Fractional;
        int o = ONE.m_raw;

        bool zeroNegative = i < 0 && f > 0;  //special handling for numbers where -1 < value < 0
        if (zeroNegative && i == -o) //if integral value is -1
        {
            i = 0; //set i to zero
            sb.Append('-'); //handling for negative zero (e.g -0.1)
        }
        sb.Append(i >> POINT).ToString(); //integral

        if (f != 0) //fractional 
        {
            sb.Append('.');
            if (zeroNegative) f = o - f; //increment + invert f if i = 0
            while (f > 0) //while we have more digits to append
            {
                f *= 10; //shift along a digit
                sb.Append((char)('0' + (f >> POINT))); //'0' + digit (0-9) = char as digit, shift f into single digit scope
                f &= MAX_FRACTIONAL; //mask out integral
            }
        }

        return sb.ToString();
    }

    //Possible TODO: trig lookup tables?


}

// todo: add license

using System.Globalization;

namespace Svg
{
    internal class CoordinateParser
    {
        private readonly string _coords;
        private CoordinateParser.NumState _currState = CoordinateParser.NumState.separator;
        private CoordinateParser.NumState _newState = CoordinateParser.NumState.separator;
        private int i;

        public int Position { get; private set; }

        public bool HasMore { get; private set; } = true;

        public CoordinateParser(string coords)
        {
            _coords = coords;
            if (string.IsNullOrEmpty(_coords))
            {
                HasMore = false;
            }

            if (!char.IsLetter(coords[0]))
            {
                return;
            }

            ++i;
        }

        private bool MarkState(bool state)
        {
            HasMore = state;
            ++i;
            return state;
        }

        public bool TryGetBool(out bool result)
        {
            for (; i < _coords.Length && HasMore; ++i)
            {
                if (_currState == CoordinateParser.NumState.separator)
                {
                    if (CoordinateParser.IsCoordSeparator(_coords[i]))
                    {
                        _newState = CoordinateParser.NumState.separator;
                    }
                    else
                    {
                        if (_coords[i] == '0')
                        {
                            result = false;
                            _newState = CoordinateParser.NumState.separator;
                            Position = i + 1;
                            return MarkState(true);
                        }
                        if (_coords[i] == '1')
                        {
                            result = true;
                            _newState = CoordinateParser.NumState.separator;
                            Position = i + 1;
                            return MarkState(true);
                        }
                        result = false;
                        return MarkState(false);
                    }
                }
                else
                {
                    result = false;
                    return MarkState(false);
                }
            }
            result = false;
            return MarkState(false);
        }

        public bool TryGetFloat(out float result)
        {
            for (; i < _coords.Length && HasMore; ++i)
            {
                switch (_currState)
                {
                    case CoordinateParser.NumState.separator:
                        if (char.IsNumber(_coords[i]))
                        {
                            _newState = CoordinateParser.NumState.integer;
                            break;
                        }
                        if (CoordinateParser.IsCoordSeparator(_coords[i]))
                        {
                            _newState = CoordinateParser.NumState.separator;
                            break;
                        }
                        switch (_coords[i])
                        {
                            case '+':
                            case '-':
                                _newState = CoordinateParser.NumState.prefix;
                                break;
                            case '.':
                                _newState = CoordinateParser.NumState.decPlace;
                                break;
                            default:
                                _newState = CoordinateParser.NumState.invalid;
                                break;
                        }
                        break;
                    case CoordinateParser.NumState.prefix:
                        _newState = !char.IsNumber(_coords[i]) ? (_coords[i] != '.' ? CoordinateParser.NumState.invalid : CoordinateParser.NumState.decPlace) : CoordinateParser.NumState.integer;
                        break;
                    case CoordinateParser.NumState.integer:
                        if (char.IsNumber(_coords[i]))
                        {
                            _newState = CoordinateParser.NumState.integer;
                            break;
                        }
                        if (CoordinateParser.IsCoordSeparator(_coords[i]))
                        {
                            _newState = CoordinateParser.NumState.separator;
                            break;
                        }
                        switch (_coords[i])
                        {
                            case '+':
                            case '-':
                                _newState = CoordinateParser.NumState.prefix;
                                break;
                            case '.':
                                _newState = CoordinateParser.NumState.decPlace;
                                break;
                            case 'E':
                            case 'e':
                                _newState = CoordinateParser.NumState.exponent;
                                break;
                            default:
                                _newState = CoordinateParser.NumState.invalid;
                                break;
                        }
                        break;
                    case CoordinateParser.NumState.decPlace:
                        if (char.IsNumber(_coords[i]))
                        {
                            _newState = CoordinateParser.NumState.fraction;
                            break;
                        }
                        if (CoordinateParser.IsCoordSeparator(_coords[i]))
                        {
                            _newState = CoordinateParser.NumState.separator;
                            break;
                        }
                        switch (_coords[i])
                        {
                            case '+':
                            case '-':
                                _newState = CoordinateParser.NumState.prefix;
                                break;
                            case 'E':
                            case 'e':
                                _newState = CoordinateParser.NumState.exponent;
                                break;
                            default:
                                _newState = CoordinateParser.NumState.invalid;
                                break;
                        }
                        break;
                    case CoordinateParser.NumState.fraction:
                        if (char.IsNumber(_coords[i]))
                        {
                            _newState = CoordinateParser.NumState.fraction;
                            break;
                        }
                        if (CoordinateParser.IsCoordSeparator(_coords[i]))
                        {
                            _newState = CoordinateParser.NumState.separator;
                            break;
                        }
                        switch (_coords[i])
                        {
                            case '+':
                            case '-':
                                _newState = CoordinateParser.NumState.prefix;
                                break;
                            case '.':
                                _newState = CoordinateParser.NumState.decPlace;
                                break;
                            case 'E':
                            case 'e':
                                _newState = CoordinateParser.NumState.exponent;
                                break;
                            default:
                                _newState = CoordinateParser.NumState.invalid;
                                break;
                        }
                        break;
                    case CoordinateParser.NumState.exponent:
                        if (char.IsNumber(_coords[i]))
                        {
                            _newState = CoordinateParser.NumState.expValue;
                            break;
                        }
                        if (CoordinateParser.IsCoordSeparator(_coords[i]))
                        {
                            _newState = CoordinateParser.NumState.invalid;
                            break;
                        }
                        switch (_coords[i])
                        {
                            case '+':
                            case '-':
                                _newState = CoordinateParser.NumState.expPrefix;
                                break;
                            default:
                                _newState = CoordinateParser.NumState.invalid;
                                break;
                        }
                        break;
                    case CoordinateParser.NumState.expPrefix:
                        _newState = !char.IsNumber(_coords[i]) ? CoordinateParser.NumState.invalid : CoordinateParser.NumState.expValue;
                        break;
                    case CoordinateParser.NumState.expValue:
                        if (char.IsNumber(_coords[i]))
                        {
                            _newState = CoordinateParser.NumState.expValue;
                            break;
                        }
                        if (CoordinateParser.IsCoordSeparator(_coords[i]))
                        {
                            _newState = CoordinateParser.NumState.separator;
                            break;
                        }
                        switch (_coords[i])
                        {
                            case '+':
                            case '-':
                                _newState = CoordinateParser.NumState.prefix;
                                break;
                            case '.':
                                _newState = CoordinateParser.NumState.decPlace;
                                break;
                            default:
                                _newState = CoordinateParser.NumState.invalid;
                                break;
                        }
                        break;
                }
                if (_newState < _currState)
                {
                    result = float.Parse(_coords.Substring(Position, i - Position), NumberStyles.Float, CultureInfo.InvariantCulture);
                    Position = i;
                    _currState = _newState;
                    return MarkState(true);
                }
                if (_newState != _currState && _currState == CoordinateParser.NumState.separator)
                {
                    Position = i;
                }

                if (_newState == CoordinateParser.NumState.invalid)
                {
                    result = float.MinValue;
                    return MarkState(false);
                }
                _currState = _newState;
            }
            if (_currState == CoordinateParser.NumState.separator || !HasMore || Position >= _coords.Length)
            {
                result = float.MinValue;
                return MarkState(false);
            }
            result = float.Parse(_coords.Substring(Position, _coords.Length - Position), NumberStyles.Float, CultureInfo.InvariantCulture);
            Position = _coords.Length;
            return MarkState(true);
        }

        private static bool IsCoordSeparator(char value)
        {
            switch (value)
            {
                case '\t':
                case '\n':
                case '\r':
                case ' ':
                case ',':
                    return true;
                default:
                    return false;
            }
        }

        private enum NumState
        {
            invalid,
            separator,
            prefix,
            integer,
            decPlace,
            fraction,
            exponent,
            expPrefix,
            expValue,
        }
    }
}

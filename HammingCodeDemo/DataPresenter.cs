using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace HammingCodeDemo
{
    internal class DataPresenter
    {
        private const float _leftRightOffset = 10f;
        private const float _inchPerEm = 72f;
        private const float _mainFontScale = 0.7f;
        private const float _captionFontScale = 0.25f;
        private const float _selectedPenWidth = 4f;
        private readonly IList<int> _controlBitIndices = new List<int>();
        private readonly IList<string> _explanation = new List<string>();
        private readonly Pen _selectedPen;
        private readonly StringFormat _stringFormatCenter;
        private readonly StringFormat _stringFormatLeft;
        private readonly IList<string> _syndromeExplanation = new List<string>();
        private readonly Pen _whitePen;
        private BitArray _bits;
        private int _blockBinNumberLength;
        private int _blockLength;
        private Font _captionFont;
        private DirectBitmap _directBitmap;
        private float _elementHeight;
        private RectangleF[] _elementRects;
        private float _elementWidth;
        private Graphics _g;
        private Font _mainFont;
        private SizeF _rectSz;
        private int _step;
        private int _width = -1, _height = -1;
        private float _y;

        public DataPresenter()
        {
            _whitePen = new Pen(Brushes.White, 2f);
            _selectedPen = new Pen(new SolidBrush(Color.Red), _selectedPenWidth);
            _stringFormatCenter = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            _stringFormatLeft = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center
            };
        }

        public void SetBlockLength(int blockLength)
        {
            _blockLength = blockLength;
            var floorLog2 = FloorLog2(blockLength);
            var delta = 0;
            if (Math.Pow(2, floorLog2) - blockLength < 0) delta = 1;
            _blockBinNumberLength = floorLog2 + delta;
            _elementRects = new RectangleF[_blockLength];
            _bits = new BitArray(_blockLength);
            _controlBitIndices.Clear();
            for (var i = 1; i < _blockLength; i++)
                if (IsControlBit(i))
                    _controlBitIndices.Add(i);
        }

        public void NextStep()
        {
            _step++;
            if (_step == 6) PrepareControlBitExplains();
            if (_step > 9) PrepareSyndromExplains();
        }

        public void PrevStep()
        {
            if (_step == 0) return;
            _step--;
        }

        public void Click(Point location)
        {
            if (_elementRects == null) return;
            if (_step == 5)
                for (var i = 0; i < _elementRects.Length; i++)
                {
                    var r = _elementRects[i];
                    if (!r.IsEmpty && r.Contains(location.X, location.Y) && !IsControlBit(i)) _bits[i] ^= true;
                }

            if (_step > 7 + _controlBitIndices.Count)
            {
                for (var i = 0; i < _elementRects.Length; i++)
                {
                    var r = _elementRects[i];
                    if (!r.IsEmpty && r.Contains(location.X, location.Y)) _bits[i] ^= true;
                }

                PrepareSyndromExplains();
            }
        }

        public void DisplayTo(Graphics graphics, int width, int height)
        {
            if (_directBitmap == null || _width != width || _height != height)
            {
                _width = width;
                _height = height;
                _elementWidth = CalcElementWidth();
                _elementHeight = _elementWidth;
                if (_directBitmap != null)
                {
                    _g.Dispose();
                    _directBitmap.Dispose();
                }

                _directBitmap = new DirectBitmap(width, height);

                _g = Graphics.FromImage(_directBitmap.Bitmap);
                _g.SmoothingMode = SmoothingMode.HighSpeed;
                CreateFonts();
            }

            _g.FillRectangle(Brushes.Black, new RectangleF(0, 0, width, height));
            _rectSz = new SizeF(_elementWidth, _elementHeight);
            _y = 2 * _captionFontScale * _elementHeight; // (height - _elementHeight) / 6;

            for (var i = 0; i < _blockLength; i++)
            {
                var x = CalcLeftOffsetForRectNumber(i);
                _elementRects[i] = new RectangleF(new PointF(x, _y), _rectSz);
            }

            _g.DrawRectangles(_whitePen, _elementRects);
            if (_step > 0) DrawHeaderCaption();
            if (_step > 1) DrawBottomCaption();
            if (_step > 2)
            {
                for (var i = 1; i < _blockLength; i++)
                {
                    if (IsControlBit(i))
                    {
                        var r = _elementRects[i];
                        _g.FillRectangle(Brushes.DarkBlue, r.X + 1, r.Y + 1, r.Width - 1 * 2, r.Height - 1 * 2);
                    }
                    else
                    {
                        if (_step > 4) DrawBitInRect(i);
                    }
                }
            }

            if (_step > 3)
            {
                var r = _elementRects[0];
                _g.FillRectangle(Brushes.Gray, r.X + 1, r.Y + 1, r.Width - 1 * 2, r.Height - 1 * 2);
            }

            if (_step > 5)
            {
                var controlBitIndexPointer = _step - 6;
                if (_step < _controlBitIndices.Count + 6)
                {
                    var controlBitIndex = _controlBitIndices[controlBitIndexPointer];
                    var r = _elementRects[controlBitIndex];
                    _g.DrawRectangle(_selectedPen, r.X + _selectedPenWidth, r.Y + _selectedPenWidth,
                        r.Width - _selectedPenWidth * 2, r.Height - _selectedPenWidth * 2);
                    for (var i = controlBitIndex + 1; i < _blockLength; i++)
                    {
                        if ((i & controlBitIndex) != 0)
                        {
                            r = _elementRects[i];
                            _g.FillRectangle(Brushes.Red, r.X + 1, r.Y + 1, r.Width - 1 * 2, r.Height - 1 * 2);
                            DrawBitInRect(i);
                        }
                    }
                }

                var n = Math.Min(controlBitIndexPointer, _explanation.Count - 1);
                for (var i = 0; i <= n; i++)
                {
                    if (_step <= 7 + _controlBitIndices.Count || _blockLength > 11)
                    {
                        var s = _explanation[i];
                        var r = CalcCaptionPositionRect(i + 3);
                        _g.DrawString(s, _captionFont, _step <= 11 ? Brushes.White : Brushes.Gray, r,
                            _stringFormatLeft);
                    }

                    DrawBitInRect(i < _controlBitIndices.Count ? _controlBitIndices[i] : 0);
                }

                if (_step > 7 + _controlBitIndices.Count)
                {
                    var offset = _blockLength > 11 ? n + 5 : 3;
                    for (var i = 0; i < _syndromeExplanation.Count; i++)
                    {
                        var r = CalcCaptionPositionRect(i + offset);
                        _g.DrawString(_syndromeExplanation[i], _captionFont, Brushes.White, r, _stringFormatLeft);
                    }
                }
            }

            graphics.DrawImage(_directBitmap.Bitmap, 0, 0);
        }

        private RectangleF CalcCaptionPositionRect(int topOffset)
        {
            var sz = new SizeF(_width - 2f * _leftRightOffset, _captionFont.Height);
            return new RectangleF(
                new PointF(_leftRightOffset, _y + 0.75f * _elementHeight + _captionFont.Height * topOffset),
                sz);
        }

        private float CalcLeftOffsetForRectNumber(int number)
        {
            return _leftRightOffset + number * _elementWidth;
        }

        private void DrawHeaderCaption()
        {
            for (var i = 0; i < _blockLength; i++)
            {
                var x = CalcLeftOffsetForRectNumber(i);
                var s = $"b{i:d}";
                _g.DrawString(s, _captionFont, Brushes.White,
                    new RectangleF(new PointF(x, _y - 0.75f * _elementHeight), _rectSz), _stringFormatCenter);
            }
        }

        private void DrawBottomCaption()
        {
            for (var i = 0; i < _blockLength; i++)
            {
                var x = CalcLeftOffsetForRectNumber(i);
                _g.DrawString(IntToBin(i, _blockBinNumberLength), _captionFont, Brushes.White,
                    new RectangleF(new PointF(x, _y + 0.75f * _elementHeight), _rectSz), _stringFormatCenter);
            }
        }

        private void DrawBitInRect(int i)
        {
            var x = _leftRightOffset + i * _elementWidth;
            var s = _bits[i] ? "1" : "0";
            _g.DrawString(s, _mainFont, Brushes.Yellow,
                new RectangleF(new PointF(x, _y), _rectSz), _stringFormatCenter);
        }

        private static bool IsControlBit(int i)
        {
            return Math.Abs(Math.Pow(2, FloorLog2(i)) - i) < 1.0;
        }

        private float CalcElementWidth()
        {
            return (_width - 2f * _leftRightOffset) / (_blockLength + 1);
        }

        private void CreateFonts()
        {
            if (_mainFont != null) _mainFont.Dispose();
            if (_captionFont != null) _captionFont.Dispose();
            _mainFont = new Font("Consolas", _elementWidth * _inchPerEm / _g.DpiX * _mainFontScale, FontStyle.Bold);
            _captionFont = new Font("Consolas", _elementWidth * _inchPerEm / _g.DpiX * _captionFontScale,
                FontStyle.Bold);
        }

        private static string IntToBin(int val, int align = 1)
        {
            var sb = new StringBuilder();
            while (val != 0)
            {
                sb.Insert(0, char.ConvertFromUtf32(val % 2 + '0'));
                val /= 2;
                align--;
            }

            for (var i = 0; i < align; i++) sb.Insert(0, '0');
            return sb.ToString();
        }

        private static int FloorLog2(int v)
        {
            var r = -1;
            while (v != 0)
            {
                v /= 2;
                r++;
            }

            return r;
        }

        private void PrepareControlBitExplains()
        {
            _explanation.Clear();
            var sb = new StringBuilder();
            foreach (var n in _controlBitIndices)
            {
                for (var i = n + 1; i < _bits.Count; i++)
                {
                    if ((i & n) != 0)
                    {
                        sb.Append($"b{i:d} ⨁ ");
                    }
                }

                if (sb.Length >= 3) sb.Remove(sb.Length - 3, 3);
                sb.Append(" = ");
                var controlBit = 0;
                for (var i = n + 1; i < _bits.Count; i++)
                {
                    if ((i & n) != 0)
                    {
                        var b = _bits[i] ? 1 : 0;
                        controlBit ^= b;
                        sb.Append($"{b:d} ⨁ ");
                    }
                }
                _bits[n] = controlBit == 1;
                if (sb.Length >= 3) sb.Remove(sb.Length - 3, 3);
                sb.Append($" = {controlBit:d}");
                sb.Insert(0, $"b{n:d} = ");
                _explanation.Add(sb.ToString());
                sb.Clear();
            }

            for (var i = 1; i < _bits.Count; i++)
            {
                sb.Append($"b{i:d} ⨁ ");
            }

            if (sb.Length >= 1)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Insert(0, "b0 = ");
            sb.Append(" =");
            _explanation.Add(sb.ToString());
            sb.Clear();
            sb.Append("     ");

            var auxControlBit = 0;
            for (var i = 1; i < _bits.Count; i++)
            {
                var b = _bits[i] ? 1 : 0;
                sb.Append($"{b:d} ⨁ ");
                auxControlBit ^= b;
            }

            if (sb.Length >= 3)
            {
                sb.Remove(sb.Length - 3, 3);
            }
            sb.Append($" = {auxControlBit:d}");

            _explanation.Add(sb.ToString());
            _bits[0] = auxControlBit == 1;
        }

        private void PrepareSyndromExplains()
        {
            _syndromeExplanation.Clear();
            var syndromeBits = new BitArray(_controlBitIndices.Count);
            var sb = new StringBuilder();
            for (var i = 0; i < _controlBitIndices.Count; i++)
            {
                var n = _controlBitIndices[i];
                for (var j = n; j < _bits.Count; j++)
                {
                    if ((j & n) != 0)
                        sb.Append($"b{j:d} ⨁ ");
                }

                if (sb.Length > 0)
                    sb.Remove(sb.Length - 3, 3);
                else continue;
                sb.Append(" = ");
                var syndromeBit = 0;
                for (var j = n; j < _bits.Count; j++)
                {
                    if ((j & n) != 0)
                    {
                        var b = _bits[j] ? 1 : 0;
                        syndromeBit ^= b;
                        sb.Append(string.Format("{0:d} ⨁ ", b));
                    }
                }

                syndromeBits[i] = syndromeBit != 0;
                if (sb.Length >= 3) sb.Remove(sb.Length - 3, 3);
                sb.Append($" = {syndromeBit:d}");
                sb.Insert(0, $"s{i:d} = ");
                _syndromeExplanation.Add(sb.ToString());
                sb.Clear();
            }

            for (var i = syndromeBits.Count - 1; i >= 0; i--)
            {
                sb.Append($"s{i:d},");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(" = ");
            for (var i = syndromeBits.Count - 1; i >= 0; i--)
            {
                sb.Append($"{(syndromeBits[i] ? 1 : 0):d},");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(" = ");
            var syndromeSum = 0;
            var power2 = 1;
            foreach (bool b in syndromeBits)
            {
                syndromeSum += b ? power2 : 0;
                power2 *= 2;
            }

            sb.Append($"{syndromeSum:d}");
            if (syndromeSum != 0) sb.Append(" !!!");
            _syndromeExplanation.Add(sb.ToString());
            sb.Clear();

            for (var i = 0; i < _bits.Count; i++)
            {
                sb.Append($"b{i:d} ⨁ ");
            }
            if (sb.Length >= 1)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append(" =");
            sb.Insert(0, "p = ");
            _syndromeExplanation.Add(sb.ToString());
            sb.Clear();
            sb.Append("    ");
            var parityBit = 0;
            for (var i = 0; i < _bits.Count; i++)
            {
                var b = _bits[i] ? 1 : 0;
                sb.Append($"{b:d} ⨁ ");
                parityBit ^= b;
            }

            if (sb.Length >= 3) sb.Remove(sb.Length - 3, 3);
            sb.Append($" = {parityBit:d}");
            if (parityBit != 0) sb.Append(" !!!");
            _syndromeExplanation.Add(sb.ToString());
        }
    }
}

using System.Diagnostics;

namespace PlusUi.core;

[DebuggerDisplay("Point: X = {X}, Y = {Y}")]
public readonly struct Point(float x, float y)
{
    public float X { get; } = x;
    public float Y { get; } = y;
}
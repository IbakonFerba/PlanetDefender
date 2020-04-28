using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para>Class Info</para>
///
/// v1.0 mm/20yy
/// Written by Fabian Kober
/// fabian-kober@gmx.net
/// </summary>
public class Grid
{
    public uint this[int i]
    {
        get => _cells[i];
        set => _cells[i] = value;
    }

    public int Size => _cells.Length;
    
    private readonly uint[] _cells;
    private GridCellObject[] _cellObjects;

    public Grid(int numberOfCells)
    {
        _cells = new uint[numberOfCells];
        _cellObjects = new GridCellObject[numberOfCells];
    }

    public void SetCellObject(int index, GridCellObject cellObject)
    {
        _cellObjects[index] = cellObject;
    }

    public GridCellObject GetCellObject(int index)
    {
        return _cellObjects[index];
    }
}

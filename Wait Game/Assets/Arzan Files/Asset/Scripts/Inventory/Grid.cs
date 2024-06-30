
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;


public class Grid
{
    private readonly int _width;
    private readonly int _height;
    private readonly int _cellSize;
    private readonly Vector2 _origin;

    private readonly Dictionary<(int, int), Vector2> coordinatePositionMapping;
    private HashSet<Vector2> unavailableCoordinates;

    public Grid(int _rowSize, int _columnSize, int _cellSize, Vector2 _origin)
    {
        this._width = _rowSize;
        this._height = _columnSize;
        this._cellSize = _cellSize;
        this._origin = _origin;
        coordinatePositionMapping = new();
        unavailableCoordinates = new();
    }

    public void CreateGrid()
    {
        for (int i = 0; i < _width / _cellSize; i++)
        {
            for (int j = 0; j < _height / _cellSize; j++)
            {

                var posVector = new Vector2(_origin.x + (i * _cellSize) + (_cellSize / 2),
                    _origin.y - (j * _cellSize) - (_cellSize / 2));

                coordinatePositionMapping.Add((i, j), posVector);
            }
        }
    }

    public Vector2[] GetPosArrayUnderItem(float minX, float minY, float maxX, float maxY, int size)
    {

        int index = -1;

        Vector2[] positionArray = new Vector2[size];

        foreach (var position in coordinatePositionMapping.Values)
        {
            if (position.x >= minX && position.x <= maxX
                && position.y >= minY && position.y <= maxY)
            {
                index++;
                positionArray[index] = position;
            }
        }

        return positionArray;
    }

    public Vector2? GetFinalAnchorPositionForItem(float minX, float minY, float maxX, float maxY, int size)
    {
        float minPosX = float.MaxValue, minPosY = float.MaxValue, maxPosX = float.MinValue, maxPosY = float.MinValue;

        Vector2[] positionArray = GetPosArrayUnderItem(minX, minY, maxX, maxY, size);

        if (positionArray[0] == Vector2.zero)
            return null;

        for (int i = 0; i < positionArray.Length; i++)
        {
            if (IsPosFilled(positionArray[i]))
            {
                Debug.Log("Is invalid");
                return null;
            }
            if (positionArray[i].x < minPosX)
                minPosX = positionArray[i].x;

            if (positionArray[i].x > maxPosX)
                maxPosX = positionArray[i].x;

            if (positionArray[i].y < minPosY)
                minPosY = positionArray[i].y;

            if (positionArray[0].y > maxPosY)
                maxPosY = positionArray[i].y;

        }

        foreach (Vector2 position in positionArray)
            AddInvalidPositionToSet(position);

        return new Vector2((minPosX + maxPosX) / 2, (minPosY + maxPosY) / 2);
    }

    public bool IsPosFilled(Vector2 position)
    {
        if (unavailableCoordinates.Contains(position))
            return true;
        return false;
    }

    public void AddInvalidPositionToSet(Vector2 position)
    {
        unavailableCoordinates.Add(position);
    }

    public void RemovePositionFromSet(Vector2 position)
    {
        unavailableCoordinates.Remove(position);
    }


    public Vector2? GetFinalAnchorPositionToLoadItem(int sizeX, int sizeY) {

        float minX = float.MaxValue;
        float minY = float.MaxValue;    
        float maxX = float.MinValue;    
        float maxY = float.MinValue;


        foreach (var position in coordinatePositionMapping)
        {
            bool breakFlag = false;
            bool breakFlagToggled = false; 

            if (IsPosFilled(position.Value))
                continue;

            for(int i = 0; i < sizeX && !breakFlag ;i++)
            {
                for(int j = 0 ; j < sizeY && !breakFlag ; j++) {

                    (int, int) coordinate = (position.Key.Item1 + i, position.Key.Item2 + j);

                    Vector2 currentCoordinatePos = coordinatePositionMapping[coordinate];

                    if (!IsPosFilled(currentCoordinatePos)){
                        if (currentCoordinatePos.x < minX) minX = currentCoordinatePos.x;
                        if (currentCoordinatePos.y < minY) minY = currentCoordinatePos.y;
                        if (currentCoordinatePos.x > maxX) maxX = currentCoordinatePos.x;
                        if (currentCoordinatePos.y > maxY) maxY = currentCoordinatePos.y;
                        
                    }
                    else { 
                        breakFlag = true;
                        breakFlagToggled = true;
                    }
                }
            }

            if (!breakFlagToggled)
            {
                return new Vector2((minX + maxX) / 2, (minY + maxY) / 2);
            }

        }

        return null;
    }


}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator
{
    /// <summary>
    /// The area in which the path cells may be placed
    /// </summary>
    private int width,height;
    private List<Vector2Int> pathCells;
    private List<Vector2Int> invalidCells;
    private List<Vector2Int> route;
    private Transform hazards;
    private int maxTimesBacktracked = 25;
    private int timesBacktracked;

    /// <summary>
    /// Constructor Function that creates a new path generator and sets its height and width. PathGenerator is NOT a Monobehavior
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public PathGenerator(int width, int height, Transform hazards)
    {
        this.width = width;
        this.height = height;
        this.hazards = hazards;
    }

    /// <summary>
    /// Determines the positions of the path and returns the positions as a Vector2Int List
    /// </summary>
    /// <returns></returns>
    public List<Vector2Int> GeneratePath()
    {
        pathCells = new List<Vector2Int>();
        invalidCells = new List<Vector2Int>();
        invalidCells = DetermineHazardPositions();

        int y = (int)(height / 2);
        int x = 0;
        int index = 0;

        // while the x value of the current path vector2int is less than the max width of the pathw
        while (x < width )
        {
            // Adds a the current Vector2Int path position
            pathCells.Add(new Vector2Int(x,y));
            index++;
            bool validMove = false;

            // Picks a new position to move to, either up down left or right one position
            int attempts = 0;
            while(!validMove && attempts < 10)
            {
                int move = Random.Range(0, 3);
                // These prevent the path from going back on itself (if last time it went right, it cant go left this time)
                if(move == 0 && !CellIsHazard(x + 1, y) || x % 2 == 0 && CellIsEmpty(x + 1, y) || x > (width - 2))
                {
                    x++;
                    validMove = true;
                }
                else if(move == 1 && CellIsEmpty(x, y + 1) && !CellIsHazard(x, y + 1) && y < (height - 3))
                {
                    y++;
                    validMove = true;
                }
                else if(move == 2 && CellIsEmpty(x, y - 1) && !CellIsHazard(x, y - 1) && y > 2)
                {
                    y--;
                    validMove = true;
                }
                attempts++;
            }
            // Backtrack a few positions if this takes to many attempts
            if (attempts >= 10)
            {
                if (timesBacktracked >= maxTimesBacktracked)
                    return null;
                int backtrackLimit = index - 5;
                if(backtrackLimit < 1) backtrackLimit = 1;
                x = pathCells[backtrackLimit].x;
                y = pathCells[backtrackLimit].y;

                for(int i = index-1; i > backtrackLimit; i--)
                {
                    index--;
                    pathCells.RemoveAt(i);
                }
                timesBacktracked++;
            }
        }

        return pathCells;
    }

    private List<Vector2Int> DetermineHazardPositions()
    {
        List<Vector2Int> hazardPositions = new List<Vector2Int>();
        List<Vector2Int> childRange = new List<Vector2Int>();
        
        foreach (Transform child in hazards)
        {
            Vector3 pos = child.position;
            Vector3 scale = child.localScale;

            Vector2Int v2Pos = new Vector2Int((int)pos.x, (int)pos.z);
            Vector2Int topLeftPos = new Vector2Int(v2Pos.x - (int)scale.x / 2, v2Pos.y + (int)scale.z / 2);
            Vector2Int bottomRightPos = new Vector2Int(v2Pos.x + (int)scale.x / 2, v2Pos.y - (int)scale.z / 2);
            //Debug.Log($"TOPLEFT X: {topLeftPos.x} BOTRIGHT X: {bottomRightPos.x} TOPLEFT Z: {topLeftPos.y} BOTRIGHT Z: {bottomRightPos.y}");

            for(int x = topLeftPos.x; x <= bottomRightPos.x; x++)
                for(int y = topLeftPos.y; y >= bottomRightPos.y; y--)
                    childRange.Add(new Vector2Int(x, y));

            hazardPositions.AddRange(childRange);
            childRange.Clear();
        }
        return hazardPositions;
    }


    /// <summary>
    /// After the path was created, GenerateCrossroads looks at path positions and finds viable locations to add a loop to the path. This doesn't allow the loop to interect with the pre-existing path. There must be a gap between the old path and the new loop.
    /// </summary>
    /// <returns></returns>
    public bool GenerateCrossroads()
    {
        for(int i = 0; i < pathCells.Count; i++)
        {
            Vector2Int pathCell = pathCells[i];
            if(pathCell.x > 3 && pathCell.x < width - 4 && pathCell.y > 2 && pathCell.y < height - 3) 
            { 
                // This is checking in a large area around the cell in question, if all of the spaces are available, a loop can be added
                // Its ugly, but not complicated
                if(CellIsEmpty(pathCell.x, pathCell.y+3) && CellIsEmpty(pathCell.x+1, pathCell.y+3) && CellIsEmpty(pathCell.x+2, pathCell.y+3)
                && CellIsEmpty(pathCell.x-1, pathCell.y+2) && CellIsEmpty(pathCell.x, pathCell.y+2) && CellIsEmpty(pathCell.x+1, pathCell.y+2) && CellIsEmpty(pathCell.x+2, pathCell.y+2) && CellIsEmpty(pathCell.x+3, pathCell.y+2)
                && CellIsEmpty(pathCell.x-1, pathCell.y+1) && CellIsEmpty(pathCell.x, pathCell.y+1) && CellIsEmpty(pathCell.x+1, pathCell.y+1) && CellIsEmpty(pathCell.x+2, pathCell.y+1) && CellIsEmpty(pathCell.x+3, pathCell.y+1)
                && CellIsEmpty(pathCell.x+1, pathCell.y) && CellIsEmpty(pathCell.x+2, pathCell.y) && CellIsEmpty(pathCell.x+3, pathCell.y)
                && CellIsEmpty(pathCell.x+1, pathCell.y-1) && CellIsEmpty(pathCell.x+2, pathCell.y-1)
                /* Now Check if the path being placed is a hazard */ && !CellIsHazard(pathCell.x + 1, pathCell.y) && !CellIsHazard(pathCell.x + 2, pathCell.y) && !CellIsHazard(pathCell.x + 2, pathCell.y + 1) && !CellIsHazard(pathCell.x + 2, pathCell.y + 2) && !CellIsHazard(pathCell.x + 1, pathCell.y + 2) && !CellIsHazard(pathCell.x, pathCell.y + 2) && !CellIsHazard(pathCell.x, pathCell.y + 1))
                {
                    pathCells.InsertRange(i + 1, new List<Vector2Int> { new Vector2Int(pathCell.x + 1, pathCell.y), new Vector2Int(pathCell.x + 2, pathCell.y), new Vector2Int(pathCell.x + 2, pathCell.y + 1), new Vector2Int(pathCell.x + 2, pathCell.y + 2), new Vector2Int(pathCell.x + 1, pathCell.y + 2), new Vector2Int(pathCell.x, pathCell.y + 2), new Vector2Int(pathCell.x, pathCell.y + 1)});
                    return true;
                }
                // Same thing, but for a loop in a different direction
                if(CellIsEmpty(pathCell.x, pathCell.y-3) && CellIsEmpty(pathCell.x+1, pathCell.y-3) && CellIsEmpty(pathCell.x+2, pathCell.y-3)
                && CellIsEmpty(pathCell.x-1, pathCell.y-2) && CellIsEmpty(pathCell.x, pathCell.y-2) && CellIsEmpty(pathCell.x+1, pathCell.y-2) && CellIsEmpty(pathCell.x+2, pathCell.y-2) && CellIsEmpty(pathCell.x+3, pathCell.y-2)
                && CellIsEmpty(pathCell.x-1, pathCell.y-1) && CellIsEmpty(pathCell.x, pathCell.y-1) && CellIsEmpty(pathCell.x+1, pathCell.y-1) && CellIsEmpty(pathCell.x+2, pathCell.y-1) && CellIsEmpty(pathCell.x+3, pathCell.y-1)
                && CellIsEmpty(pathCell.x+1, pathCell.y) && CellIsEmpty(pathCell.x+2, pathCell.y) && CellIsEmpty(pathCell.x+3, pathCell.y)
                && CellIsEmpty(pathCell.x+1, pathCell.y+1) && CellIsEmpty(pathCell.x+2, pathCell.y+1) 
                /* Now Check if the path being placed is a hazard */ && !CellIsHazard(pathCell.x+1, pathCell.y) && !CellIsHazard(pathCell.x+2, pathCell.y) && !CellIsHazard(pathCell.x+2, pathCell.y-1) && !CellIsHazard(pathCell.x + 2, pathCell.y-2) && !CellIsHazard(pathCell.x+1, pathCell.y-2) && !CellIsHazard(pathCell.x, pathCell.y-2) && !CellIsHazard(pathCell.x, pathCell.y-1))
                {
                    pathCells.InsertRange(i + 1, new List<Vector2Int> { new Vector2Int(pathCell.x + 1, pathCell.y), new Vector2Int(pathCell.x + 2, pathCell.y), new Vector2Int(pathCell.x + 2, pathCell.y - 1), new Vector2Int(pathCell.x + 2, pathCell.y - 2), new Vector2Int(pathCell.x + 1, pathCell.y - 2), new Vector2Int(pathCell.x, pathCell.y - 2), new Vector2Int(pathCell.x, pathCell.y - 1)});
                    return true;
                }
            }
        }
        return false;
    }
    /// <summary>
    /// This creates the positions that will be used by the wave manager for enemy ai pathing.
    /// </summary>
    /// <returns></returns>
    public List<Vector2Int> GenerateRoute()
    {
        Vector2Int direction = Vector2Int.right;
        route = new List<Vector2Int>();
        Vector2Int currentCell = pathCells[0];

        while(currentCell.x < width)
        {
            route.Add(new Vector2Int(currentCell.x, currentCell.y));

            if(CellIsTaken(currentCell + direction))
            {
                currentCell += direction;
            }
            else if(CellIsTaken(currentCell + Vector2Int.up) && direction != Vector2Int.down)
            {
                direction = Vector2Int.up;
                currentCell += direction;
            }
            else if(CellIsTaken(currentCell + Vector2Int.down) && direction != Vector2Int.up)
            {
                direction = Vector2Int.down;
                currentCell += direction;
            }
            else if(CellIsTaken(currentCell + Vector2Int.left) && direction != Vector2Int.right)
            {
                direction = Vector2Int.left;
                currentCell += direction;
            }
            else if(CellIsTaken(currentCell + Vector2Int.right) && direction != Vector2Int.left)
            {
                direction = Vector2Int.right;
                currentCell += direction;
            }
            else
            {
                // No where to go, return route
                return route;
            }
        }
        return route;

        
    }
    public bool CellIsHazard(int x, int y)
    { 
        return invalidCells.Contains(new Vector2Int(x, y));
    }
    public bool CellIsEmpty(int x, int y)
    {
        return !pathCells.Contains(new Vector2Int(x, y));
    }

    public bool CellIsTaken(int x, int y)
    {
        return pathCells.Contains(new Vector2Int(x, y));
    }
    public bool CellIsTaken(Vector2Int cell)
    {
        return pathCells.Contains(cell);
    }

    /// <summary>
    /// Checks in four directions. It adds an int value to the returnValue if there is a path in the direction checked. The return value is used to determine what GridCell path should be placed. ie. Corner, straight, etc.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int GetCellNeighborValue(int x, int y)
    {
        int returnValue = 0;
        if(CellIsTaken(x, y-1)) returnValue += 1;
        if(CellIsTaken(x-1, y)) returnValue += 2; //       8
        if(CellIsTaken(x+1, y)) returnValue += 4; //     2 C 4
        if(CellIsTaken(x, y+1)) returnValue += 8; //       1
        return returnValue;
    }
}

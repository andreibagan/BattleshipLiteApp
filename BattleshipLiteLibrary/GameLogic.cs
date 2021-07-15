using BattleshipLiteLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleshipLiteLibrary
{
    public static class GameLogic
    {
        public static void InitializeGrid(PlayerInfoModel model)
        {
            List<string> letters = new List<string>
            {
                "A",
                "B",
                "C",
                "D",
                "E"
            };

            List<int> numbers = new List<int>
            {
                1,
                2,
                3,
                4,
                5
            };

            foreach (string letter in letters)
            {
                foreach (int number in numbers)
                {
                    AddGridSpot(model, letter, number);
                }
            }
        } //norm

        public static bool PlayerStillActive(PlayerInfoModel player)
        {
            bool isActive = false;

            foreach (var ship in player.ShipLocations)
            {
                if (ship.Status != GridSpotStatus.Sunk)
                {
                    isActive = true;
                    break;
                }
            }

            return isActive;
        } //norm

        private static void AddGridSpot(PlayerInfoModel modal, string letter, int number)
        {
            GridSpotModel spot = new GridSpotModel
            {
                SpotLetter = letter,
                SpotNumber = number,
                Status = GridSpotStatus.Empty
            };

            modal.ShotGrid.Add(spot);
        } //norm

        public static int GetShotCount(PlayerInfoModel player) //norm
        {
            int shotCount = 0;

            foreach (var shot in player.ShotGrid)
            {
                if (shot.Status != GridSpotStatus.Empty)
                {
                    shotCount++;
                }
            }

            return shotCount;
        }

        public static bool PlaceShip(PlayerInfoModel model, string location) //norm
        {
            bool output = false;
            var splitLocation = SplitShotIntoRowAndColumn(location);

            bool isValidLocation = ValidateGridLocation(model, splitLocation);
            bool isSpotOpen = ValidateShipLocation(model, splitLocation);

            if (isValidLocation && isSpotOpen)
            {
                GridSpotModel gridSpot = new GridSpotModel
                {
                    SpotLetter = splitLocation.row.ToUpper(),
                    SpotNumber = splitLocation.column,
                    Status = GridSpotStatus.Ship
                };

                model.ShipLocations.Add(gridSpot);
                output = true; 
            }

            return output;
        }

        private static bool ValidateShipLocation(PlayerInfoModel model, (string row, int column) splitLocation) //norm
        {
            bool isValidLocation = true;

            foreach (var ship in model.ShipLocations)
            {
                if (ship.SpotLetter == splitLocation.row.ToUpper() && ship.SpotNumber == splitLocation.column)
                {
                    isValidLocation = false;
                    break;
                }
            }

            return isValidLocation;
        }

        private static bool ValidateGridLocation(PlayerInfoModel model, (string row, int column) splitLocation)
        {
            bool isValidLocation = false;

            foreach (var ship in model.ShotGrid)
            {
                if (ship.SpotLetter == splitLocation.row.ToUpper() && ship.SpotNumber == splitLocation.column)
                {
                    isValidLocation = true;
                    break;
                }
            }

            return isValidLocation;
        } //norm

        public static bool ValidateShot(PlayerInfoModel player, string row, int column)
        {
            bool isValidShot = false;

            foreach (var gridSpot in player.ShotGrid)
            {
                if (gridSpot.SpotLetter == row.ToUpper() && gridSpot.SpotNumber == column)
                {
                    if (gridSpot.Status == GridSpotStatus.Empty)
                    {
                        isValidShot = true;
                        break;
                    } 
                }
            }

            return isValidShot;
        } //norm

        public static (string row, int column) SplitShotIntoRowAndColumn(string shot)
        {
            string shotRow = String.Empty;
            int shotColumn = 0;

            if (shot.Length != 2)
            {
                throw new ArgumentException("This was an invalid shot type.", "shot");
            }

            char[] shotArray = shot.ToArray();

            shotRow = shotArray[0].ToString();
            shotColumn = Int32.Parse(shotArray[1].ToString());

            return (shotRow, shotColumn);
        } //norm

        public static bool IdentifyShotResult(PlayerInfoModel opponent, string row, int column)
        {
            bool isAhit = false;

            foreach (var ship in opponent.ShipLocations)
            {
                if (ship.SpotLetter == row.ToUpper() && ship.SpotNumber == column)
                {
                    isAhit = true;
                    ship.Status = GridSpotStatus.Sunk;
                }
            }

            return isAhit;
        }

        public static void MarkShotResult(PlayerInfoModel player, string row, int column, bool isAHit)
        {
            foreach (var gridSpot in player.ShotGrid)
            {
                if (gridSpot.SpotLetter == row.ToUpper() && gridSpot.SpotNumber == column)
                {
                    if (isAHit)
                    {
                        gridSpot.Status = GridSpotStatus.Hit;
                    }
                    else
                    {
                        gridSpot.Status = GridSpotStatus.Miss;
                    }
                }
            }
        }
    }
}

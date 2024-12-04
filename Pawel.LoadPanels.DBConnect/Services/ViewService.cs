using BowlingSys.DBConnect;
using BowlingSys.Entities.UserDBEntities;
using System.Data.SqlClient;
using System.Data;
using Npgsql;
using AutoMapper;
using System;
using System.Text.Json;

namespace BowlingSys.Services.UserService
{
    public class ViewService
    {
        private DBConnect.DBConnect _DBConnect;

        public ViewService( DBConnect.DBConnect dBConnect)
        {
            _DBConnect = dBConnect;
        }

        public async Task<GetViews> CallGetAllPanelViews()
        {
            var finalResults = new GetViews { Results = new List<GetPanelView>() };

            try
            {
                var result = (List<Dictionary<string, object>>)await _DBConnect.SelectAndRunFunction("SELECT * FROM views.getallpanelviews()");

                foreach (var row in result)
                {
                    var panelView = new GetPanelView
                    {
                        Title = row.GetValueOrDefault("title")?.ToString() ?? string.Empty,
                        Image = row.GetValueOrDefault("image")?.ToString() ?? string.Empty,
                        Description = row.GetValueOrDefault("description")?.ToString() ?? string.Empty,
                        ExtendedDescription = row.GetValueOrDefault("extended_description")?.ToString() ?? string.Empty,
                        Price = row.GetValueOrDefault("price")?.ToString() ?? string.Empty
                    };

                    finalResults.Results.Add(panelView);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching panel view details: {ex.Message}");

                throw new ApplicationException("Failed to retrieve panel view details. Please try again later.", ex);
            }

            return finalResults;
        }





    }
}

// https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-5.0&tabs=visual-studio
// https://entityframework.net/linq-queries

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BapApi.Models;
using System.Net.Http;

namespace BapApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreAppsController : ControllerBase
    {
        private readonly StoreAppsContext _context;

        public StoreAppsController(StoreAppsContext context)
        {
            _context = context;
        }

        // GET: api/StoreApps (StoreApps as in StoreAppsController, line 17)
        // Get all the data from the database
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<StoreAppDTO>>> GetStoreApps()
        {
            return await _context.StoreApps.Select(x => StoreAppToDTO(x)).ToListAsync();
        } 

        // GET: api/StoreApps/1
        // Get a single row from the database by Id
        [HttpGet("{id}")]
        public async Task<ActionResult<StoreAppDTO>> GetStoreApp(int id)
        {
            var storeApp = await _context.StoreApps.FindAsync(id);

            if (storeApp == null)
            {
                return NotFound();
            }

            return StoreAppToDTO(storeApp);
        }

        // GET: api/StoreApps/FirstTen
        // Get the first ten results from the database aftering ordering the data by Id
        [HttpGet("FirstTen")]
        public async Task<ActionResult<IEnumerable<StoreAppDTO>>> GetStoreTopTen()
        {

            var storeTopTen = await _context.StoreApps.Select(x => StoreAppToDTO(x)).Take(10).ToListAsync();

            if (storeTopTen == null)
            {
                return NotFound();
            }

            return storeTopTen;
        }

        [HttpGet("TopTenApps")]
        public async Task<ActionResult<IEnumerable<StoreApp>>> GetStoreTopTenApps()
        {

            var storeTopTenApps = await _context.StoreApps.OrderByDescending(x => x.Rating).ThenByDescending(x =>x.People).Take(10).ToListAsync();

            if (storeTopTenApps == null)
            {
                return NotFound();
            }

            return storeTopTenApps;
        }

        [HttpGet("Sort/dsc")]
        public async Task<ActionResult<IEnumerable<StoreApp>>> GetStoreSortDSC(string column)
        {
            var storeTopTenApps = new List<StoreApp>();

            if (column == "rating")
            {
                storeTopTenApps = await _context.StoreApps.OrderByDescending(x => x.Rating).ToListAsync();
            }

            if (column == "name")
            {
                storeTopTenApps = await _context.StoreApps.OrderByDescending(x => x.Name).ToListAsync();
            }

            if (column == "id")
            {
                 storeTopTenApps = await _context.StoreApps.OrderByDescending(x => x.Id).ToListAsync();
            }

            if (column == "price")
            {
                storeTopTenApps = await _context.StoreApps.OrderByDescending(x => x.Price).ToListAsync();
            }

            if (column == "people")
            {
                storeTopTenApps = await _context.StoreApps.OrderByDescending(x => x.People).ToListAsync();
            }
            if (column == "category")
            {
                storeTopTenApps = await _context.StoreApps.OrderByDescending(x => x.Category).ToListAsync();
            }


             if (storeTopTenApps == null)
             {
                 return NotFound();
             }

            return storeTopTenApps;

           
        }


        [HttpGet("Sort/asc")]
        public async Task<ActionResult<IEnumerable<StoreApp>>> GetStoreSortASC(string column)
        {
            var storeTopTenApps = new List<StoreApp>();

            if (column == "rating")
            {
                storeTopTenApps = await _context.StoreApps.OrderBy(x => x.Rating).ToListAsync();
            }

            if (column == "name")
            {
                storeTopTenApps = await _context.StoreApps.OrderBy(x => x.Name).ToListAsync();
            }

            if (column == "id")
            {
                storeTopTenApps = await _context.StoreApps.OrderBy(x => x.Id).ToListAsync();
            }

            if (column == "price")
            {
                storeTopTenApps = await _context.StoreApps.OrderBy(x => x.Price).ToListAsync();
            }

            if (column == "people")
            {
                storeTopTenApps = await _context.StoreApps.OrderBy(x => x.People).ToListAsync();
            }
            if (column == "category")
            {
                storeTopTenApps = await _context.StoreApps.OrderBy(x => x.Category).ToListAsync();
            }

            if (storeTopTenApps == null)
            {
                return NotFound();
            }

            return storeTopTenApps;


        }

        //Get page 
        [HttpGet("GetPage/{start}")]
        public async Task<ActionResult<IEnumerable<StoreAppDTO>>> GetPageSet(int start)
        {
            var pageSet = await _context.StoreApps.Select(x => StoreAppToDTO(x)).Skip(start).Take(25).ToListAsync();

            if (pageSet == null)
            {
                return NotFound();
            }

            return pageSet;
        }

        //Gets Ratings for barchart
        [HttpGet("BarChart")]
        public IEnumerable<BarChartValues> GetBarChart(string category)
        {
            var pageSet = _context.StoreApps.Where(q => q.Category.ToUpper() == category.ToUpper())
                                            .GroupBy(q => q.Rating)
                                            .Select(group => new BarChartValues{Value = group.Key
                                            .ToString(), Count = group.Count()});

            if (pageSet == null)
            {
                //return NotFound();
            }

            return pageSet;
        }

        [HttpGet("DataSize")]
        public int GetDataSize()
        {
            var count = _context.StoreApps.Count();

            return count;
        }



        //Get list of categories
        [HttpGet("CategoryNames")]
        public IEnumerable<NameModel> GetCategoryNames()
        {
            var pageSet = _context.StoreApps.GroupBy(q => q.Category)
                                            .Select(group => new NameModel { Name = group.Key.ToString() });
                                            
            if (pageSet == null)
            {
                //return NotFound();
            }

            return pageSet;
        }

        //API get search results
        [HttpGet("Search")]
        public async Task<ActionResult<StoreAppDTO>> GetSearchApp(string SearchTerm)
        {
            var lowerCaseSearchTerm = SearchTerm.Trim().ToLower();
            var searchApp = await _context.StoreApps
                .Where(a => a.Name.ToLower()
                .Contains(lowerCaseSearchTerm) || a.Category.ToLower().Contains(lowerCaseSearchTerm))
                .Take(100).ToListAsync();

            if (searchApp == null)
            {
                return NotFound();
            }

            return Ok(searchApp);
        }


        // POST: api/StoreApps
        // Add a new record to the database
        [HttpPost]
        public async Task<ActionResult<StoreAppDTO>> PostAddApp(StoreAppDTO storeAppDTO)
        {
            //var todoItem = new StoreAppDTO
            var storeApp = new StoreApp
            {
                Id = storeAppDTO.Id,
                Name = storeAppDTO.Name,
                Rating = storeAppDTO.Rating,
                People = storeAppDTO.People,
                Category = storeAppDTO.Category,
                Date = storeAppDTO.Date,
                Price = storeAppDTO.Price

            };

            _context.StoreApps.Add(storeApp);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetStoreApp),
                new { id = storeApp.Id },
                storeApp);
        }

        // Delete: api/StoreApps/1
        // Delete a single row from the database by Id
       [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStoreApps(long id)
        {
            var StoreApps = await _context.StoreApps.FindAsync(id);
            if (StoreApps == null)
            {
                return NotFound();
            }
            _context.StoreApps.Remove(StoreApps);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // DTO helper method. "Production apps typically limit the data that's input and returned using a subset of the model"
        private static StoreAppDTO StoreAppToDTO(StoreApp storeApp) =>
            new StoreAppDTO
            {
                Id = storeApp.Id,
                Name = storeApp.Name,
                Rating = storeApp.Rating,
                People = storeApp.People,
                Category = storeApp.Category,
                Date = storeApp.Date,
                Price = storeApp.Price
            };

    }

}

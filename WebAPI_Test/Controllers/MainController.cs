using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.RegularExpressions;
using WebAPI_Test.DTO;
using WebAPI_Test.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI_Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly WebApiTestDbContext _WebApiTestDbContext;
        private readonly IMapper _mapper;

        public MainController(WebApiTestDbContext webApiTestDbContext, IMapper mapper)
        {
            _WebApiTestDbContext = webApiTestDbContext;
            _mapper = mapper;
        }

        //LINQ語法參考文獻:https://vocus.cc/article/6321b307fd897800011ff10e


        // IEnumerable: 常用於返回集合類型的資料 IEnumerator:為集合逐一遍歷資料
        // IQueryable: 代表一個查詢，而不是具體的資料集合，無法用在ActionResult底下，除非轉換成List集合型態。

        /* IQueryable: 是用於定義 LINQ 查詢的介面，它提供了在資料庫或其他資料來源中進行過濾、排序和投影等操作的能力。
         然而，直接將 IQueryable 作為 ActionResult 的返回類型可能會導致問題，因為在執行查詢之前，資料庫連接可能已經關閉，
         或者查詢可能需要其他處理步驟。 */
        // GET: api/<MainController>   如果Get_Parameter的變數Name及setDate需要給值時，api/Main?Name=邱冠為&setDate=2023-06-02-2023-07-31
        [HttpGet]
        public ActionResult<IEnumerable<TodoList_SelectDTO>> Get([FromQuery] Get_Parameter _Parameter)  
        {
            var GetData = _WebApiTestDbContext.Information.ToList();  //直接搜尋總筆資料

            //DTO方法
            var Result1 = (from a in GetData
                           select new TodoList_SelectDTO
                           {
                               Id = a.Id,
                               ColName = a.ColName,
                               ColAge = a.ColAge,
                               ColAddress = a.ColAddress,
                               ColProfession = a.ColProfession,
                               DateTime = a.DateTime,
                               Upload = (from b in _WebApiTestDbContext.Statuses
                                         where b.ColAge == a.ColAge
                                         select new UploadDataDTO
                                         {
                                             ColAge = b.ColAge,
                                             ColPosition = b.ColPosition,
                                             ColNote = b.ColNote
                                         }).ToList()

                           });

            //var Result1 = (from a in _WebApiTestDbContext.Information
            //              join b in _WebApiTestDbContext.Statuses on a.ColAge equals b.ColAge
            //              select new TodoList_SelectDTO
            //              {
            //                  Id = a.Id,
            //                  ColName = a.ColName,
            //                  ColAge = a.ColAge,
            //                  ColAddress = a.ColAddress,
            //                  ColProfession = a.ColProfession,
            //                  ColPosition = b.ColPosition,
            //                  ColNote = b.ColNote,
            //                  DateTime = a.DateTime
            //              });


            //var Result2 = (from a in _WebApiTestDbContext.Information
            //              where  a.ColName == "邱冠為" 
            //              select a).ToList();

            //var Result3 = _WebApiTestDbContext.Information.Where(e => e.ColName == "李大奔").ToList();

            if (Result1 == null || Result1.Count() == 0)
            {
                return NotFound("抓取資料為空!");
            }
            else
            {
                if(!String.IsNullOrEmpty(_Parameter.Name))
                {
                    Result1 = Result1.Where(a => a.ColName == _Parameter.Name);
                   
                }
                if(_Parameter.setDate != null)
                {
                    string pattern = @"(\d{4}-\d{2}-\d{2})-(\d{4}-\d{2}-\d{2})";

                    Match match = Regex.Match(_Parameter.setDate, pattern);
                    
                    if (match.Success)
                    {
                        _Parameter.Mindate = DateTime.Parse(match.Groups[1].Value).Date;  //抓起始日期
                        _Parameter.Maxdate = DateTime.Parse(match.Groups[2].Value).Date;  //抓結束日期
                    }
                    Result1 = Result1.Where(a => a.DateTime.Date >= _Parameter.Mindate && a.DateTime.Date <= _Parameter.Maxdate);
                }
            }

            return Result1.ToList();
        }

        // GET api/<MainController>/邱冠為
        // ActionResult:指定回傳類型 IActionResult:不指定回傳類型
        [HttpGet("{sName}")]
        public ActionResult<TodoList_SelectDTO> Get([FromRoute] string sName)
        {
            //var Result = _WebApiTestDbContext.Information.Find(Id);

            var Result = (from a in _WebApiTestDbContext.Information
                           join b in _WebApiTestDbContext.Statuses on a.ColAge equals b.ColAge
                           where a.ColName == sName
                          select new TodoList_SelectDTO
                           {
                               Id = a.Id,
                               ColName = a.ColName,
                               ColAge = a.ColAge,
                               ColAddress = a.ColAddress,
                               ColProfession = a.ColProfession,
                               ColPosition = b.ColPosition,
                               ColNote = b.ColNote,
                               DateTime = a.DateTime
                          }).SingleOrDefault();  //設定成只能回傳單一一筆資料，當查詢資料為多筆或查無時，回傳NULL NoContent()

            if (Result == null)
            {
                return NoContent();
            }

            return Result;
        }

        // POST api/<MainController>
        [HttpPost]
        public ActionResult<TodoList_SelectDTO> Post([FromBody] TodoList_SelectDTO value)
        {
            Information tableA = new Information
            {
                ColName = value.ColName,
                ColAge = value.ColAge,
                ColAddress = value.ColAddress,
                ColProfession = value.ColProfession,
                DateTime = DateTime.Now
            };

            Status tableB = new Status
            {
                ColAge = value.ColAge,
                ColPosition = value.ColPosition,
                ColNote = value.ColNote
            };


            _WebApiTestDbContext.Information.Add(tableA);
            _WebApiTestDbContext.Statuses.Add(tableB);
            _WebApiTestDbContext.SaveChanges();

            return Get(value.ColName);
        }

        // PUT api/<MainController>/5
        [HttpPut("{id}")]
        public IActionResult Put([FromRoute] int id, [FromBody] Information value)
        {
            if (id != value.Id)
            {
                return BadRequest();
            }

            value.DateTime = DateTime.Now;           
            _WebApiTestDbContext.Entry(value).State = EntityState.Modified;

            try
            {
                _WebApiTestDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                if (!_WebApiTestDbContext.Information.Any(e => e.Id == id))  //Any: 函數用於檢查集合中是否存在滿足特定條件的元素，如果存在至少一個滿足條件的元素，Any 函數將返回 true，否則返回 false
                {
                    return NotFound();
                }
                else
                {
                    return StatusCode(500, "更新發生錯誤:" + ex.Message);
                }
            }
            return StatusCode(201, "更新成功!");
        }

        // DELETE api/<MainController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var result = _WebApiTestDbContext.Information.Find(id);

            if (result == null)
            {
                return NotFound();
            }

            _WebApiTestDbContext.Information.Remove(result);
            _WebApiTestDbContext.SaveChanges();

            return NoContent();
        }
    }
}

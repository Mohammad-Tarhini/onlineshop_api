using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using onlineshopowner_api.Domain.Constant;
using System.Data.SqlClient;
using onlineshopowner_api.Domain.Interfaces.IRepository;

namespace onlineshopowner_api.Infrastructure.Repositories
{
    public class CategoryRepository: IcategoryRepository
    {
        private readonly string _connectionstring;
        public CategoryRepository(string connectionstring)
        {
            _connectionstring = connectionstring;
        }

        public async Task<ResultCheckdb<Domain.Entities.Category>> checkIfCategoryExist(string categoryname)
        {
            if (string.IsNullOrWhiteSpace(categoryname))
            {
                return new ResultCheckdb<Domain.Entities.Category>
                {
                    IsSuccess = false,
                    Error = "no input ",
                };
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionstring))
                {
                    connection.Open();
                    string query = "select top 1 * from category where name=@categoryname";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@categoryname", categoryname);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new ResultCheckdb<Domain.Entities.Category>
                                {
                                    IsSuccess = true,
                                    IsFound = true,
                                };
                            }
                            return new ResultCheckdb<Domain.Entities.Category>
                            {
                                IsSuccess = true,
                                IsFound = false,
                            };
                            connection.Close();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                return new ResultCheckdb<Domain.Entities.Category>
                {
                    IsSuccess = false,
                    Error=ex.Message,

                };
            }
        }
        public async Task<UpdateDataProcess> Addcategory(string categoryname)
        {
            if (!string.IsNullOrWhiteSpace(categoryname)) { return UpdateDataProcess.yourdatanull; }
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionstring))
                {
                    connection.Open();
                    string query = "insert into category values (@name)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", categoryname);
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                        return UpdateDataProcess.Success;
                    }
                }

            }
            catch (Exception ex)
            {
                return UpdateDataProcess.catchError;
            }


        }
    }
}

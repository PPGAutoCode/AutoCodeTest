
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ProjectName.Interfaces;
using ProjectName.Types;
using ProjectName.ControllersExceptions;

namespace ProjectName.Services
{
    public class FileService : IFileService
    {
        private readonly IDbConnection _dbConnection;

        public FileService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> CreateFile(CreateFileDto createFileDto)
        {
            if (createFileDto == null || string.IsNullOrEmpty(createFileDto.FileName) || createFileDto.FileUrl == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var file = new File
            {
                Id = Guid.NewGuid(),
                FileName = createFileDto.FileName,
                FileUrl = createFileDto.FileUrl,
                Timestamp = DateTime.UtcNow
            };

            const string sql = "INSERT INTO Files (Id, FileName, FileUrl, Timestamp) VALUES (@Id, @FileName, @FileUrl, @Timestamp)";
            var affectedRows = await _dbConnection.ExecuteAsync(sql, file);

            if (affectedRows == 0)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return file.Id.ToString();
        }

        public async Task<File> GetFile(FileRequestDto fileRequestDto)
        {
            if (fileRequestDto == null || fileRequestDto.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string sql = "SELECT * FROM Files WHERE Id = @Id";
            var file = await _dbConnection.QuerySingleOrDefaultAsync<File>(sql, new { Id = fileRequestDto.Id });

            if (file == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            return file;
        }

        public async Task<string> UpdateFile(UpdateFileDto updateFileDto)
        {
            if (updateFileDto == null || updateFileDto.Id == Guid.Empty || string.IsNullOrEmpty(updateFileDto.FileName) || updateFileDto.FileUrl == null)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM Files WHERE Id = @Id";
            var existingFile = await _dbConnection.QuerySingleOrDefaultAsync<File>(selectSql, new { Id = updateFileDto.Id });

            if (existingFile == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            existingFile.FileName = updateFileDto.FileName;
            existingFile.FileUrl = updateFileDto.FileUrl;
            existingFile.Timestamp = DateTime.UtcNow;

            const string updateSql = "UPDATE Files SET FileName = @FileName, FileUrl = @FileUrl, Timestamp = @Timestamp WHERE Id = @Id";
            var affectedRows = await _dbConnection.ExecuteAsync(updateSql, existingFile);

            if (affectedRows == 0)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return existingFile.Id.ToString();
        }

        public async Task<bool> DeleteFile(DeleteFileDto deleteFileDto)
        {
            if (deleteFileDto == null || deleteFileDto.Id == Guid.Empty)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            const string selectSql = "SELECT * FROM Files WHERE Id = @Id";
            var existingFile = await _dbConnection.QuerySingleOrDefaultAsync<File>(selectSql, new { Id = deleteFileDto.Id });

            if (existingFile == null)
            {
                throw new TechnicalException("DP-404", "Technical Error");
            }

            const string deleteSql = "DELETE FROM Files WHERE Id = @Id";
            var affectedRows = await _dbConnection.ExecuteAsync(deleteSql, new { Id = deleteFileDto.Id });

            if (affectedRows == 0)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return true;
        }

        public async Task<List<File>> GetListFile(ListFileRequestDto listFileRequestDto)
        {
            if (listFileRequestDto == null || listFileRequestDto.PageLimit <= 0 || listFileRequestDto.PageOffset < 0)
            {
                throw new BusinessException("DP-422", "Client Error");
            }

            var sql = "SELECT * FROM Files";
            if (!string.IsNullOrEmpty(listFileRequestDto.SortField) && !string.IsNullOrEmpty(listFileRequestDto.SortOrder))
            {
                sql += $" ORDER BY {listFileRequestDto.SortField} {listFileRequestDto.SortOrder}";
            }
            sql += " OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";

            var files = await _dbConnection.QueryAsync<File>(sql, new { Offset = listFileRequestDto.PageOffset, Limit = listFileRequestDto.PageLimit });

            if (files == null)
            {
                throw new TechnicalException("DP-500", "Technical Error");
            }

            return files.AsList();
        }
    }
}

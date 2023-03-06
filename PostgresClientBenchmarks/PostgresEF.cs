using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class PostgresEF
{
	private SchoolContext _db;

	public PostgresEF()
	{
		_db = new SchoolContext();
	}

	~PostgresEF()
	{
		_db.Dispose();
	}


	public async Task Insert(Teacher teacher)
	{
		await _db.Teachers.AddAsync(teacher).ConfigureAwait(false);
		await _db.SaveChangesAsync().ConfigureAwait(false);
	}

	public async Task<Teacher> GetById(int id)
	{
		using (var db = new SchoolContext())
		{
			return await db.Teachers.FindAsync(id).ConfigureAwait(false);
		}
	}

	public async Task<List<Teacher>> GetBySubject(string subject)
	{
		return await _db.Teachers.Where(t => t.Subject == subject).ToListAsync().ConfigureAwait(false);
	}

	public async Task UpdateAllValues(int id, Teacher teacher)
	{
		var t = await _db.Teachers.FindAsync(id).ConfigureAwait(false);

		teacher.Id = t.Id;
		_db.Entry(t).CurrentValues.SetValues(teacher);

		await _db.SaveChangesAsync().ConfigureAwait(false);
	}

	public async Task UpdateSalary(int id, int newSalary)
	{
		var t = await _db.Teachers.FindAsync(id).ConfigureAwait(false);
		t.Salary = newSalary;
		await _db.SaveChangesAsync().ConfigureAwait(false);
	}

	public async Task UpdateLastNameById(int id, string newLastName)
	{
		var t = await _db.Teachers.FindAsync(id).ConfigureAwait(false);
		t.LastName = newLastName;
		await _db.SaveChangesAsync().ConfigureAwait(false);
	}
}

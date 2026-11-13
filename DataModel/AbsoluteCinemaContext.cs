using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace zalupa.DataModel;

public partial class AbsoluteCinemaContext : DbContext
{
    public AbsoluteCinemaContext()
    {
    }

    public AbsoluteCinemaContext(DbContextOptions<AbsoluteCinemaContext> options)
        : base(options)
    {
    }

    public async Task<List<object>> GetListByTypeAsync(Type entityType)
    {
        // 1. Берем стандартный метод Set<T>() у DbContext и подставляем в него ваш Type
        var setMethod = typeof(AbsoluteCinemaContext)
            .GetMethod(nameof(Set), Type.EmptyTypes)!
            .MakeGenericMethod(entityType);

        var dbSet = setMethod.Invoke(this, null)!;

        // 2. Вызываем стандартный ToListAsync() из Entity Framework для этого DbSet
        var toListMethod = typeof(EntityFrameworkQueryableExtensions)
            .GetMethod(nameof(EntityFrameworkQueryableExtensions.ToListAsync))!
            .MakeGenericMethod(entityType);

        var task = (Task)toListMethod.Invoke(null, new object?[] { dbSet, CancellationToken.None })!;
        await task;

        // 3. Достаем готовый список из результата задачи
        var result = task.GetType().GetProperty("Result")!.GetValue(task);
        return ((IEnumerable)result!).Cast<object>().ToList();
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Discountgroup> Discountgroups { get; set; }

    public virtual DbSet<Film> Films { get; set; }

    public virtual DbSet<Hall> Halls { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<Ticketdiscount> Ticketdiscounts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("host=localhost;Username=postgres;Password=root;database=AbsoluteCinema;Port=5432");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("client_pkey");

            entity.ToTable("client");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .HasColumnName("firstname");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(50)
                .HasColumnName("patronymic");
            entity.Property(e => e.Secondname)
                .HasMaxLength(50)
                .HasColumnName("secondname");
        });

        modelBuilder.Entity<Discountgroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("discountgroups_pkey");

            entity.ToTable("discountgroups");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Agemax).HasColumnName("agemax");
            entity.Property(e => e.Agemin).HasColumnName("agemin");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Film>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("film_pkey");

            entity.ToTable("film");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Hall>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("hall_pkey");

            entity.ToTable("hall");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Hallcount).HasColumnName("hallcount");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("session_pkey");

            entity.ToTable("session");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Baseticketcost).HasColumnName("baseticketcost");
            entity.Property(e => e.Filmid).HasColumnName("filmid");
            entity.Property(e => e.Hallid).HasColumnName("hallid");

            entity.HasOne(d => d.Film).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.Filmid)
                .HasConstraintName("session_filmid_fkey");

            entity.HasOne(d => d.Hall).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.Hallid)
                .HasConstraintName("session_hallid_fkey");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ticket_pkey");

            entity.ToTable("ticket");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Clientid).HasColumnName("clientid");
            entity.Property(e => e.Seat)
                .ValueGeneratedOnAdd()
                .HasColumnName("seat");
            entity.Property(e => e.Sessionid).HasColumnName("sessionid");

            entity.HasOne(d => d.Client).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.Clientid)
                .HasConstraintName("ticket_clientid_fkey");

            entity.HasOne(d => d.Session).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.Sessionid)
                .HasConstraintName("ticket_sessionid_fkey");
        });

        modelBuilder.Entity<Ticketdiscount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ticketdiscount_pkey");

            entity.ToTable("ticketdiscount");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Discountgroupid).HasColumnName("discountgroupid");
            entity.Property(e => e.Discountpercent).HasColumnName("discountpercent");

            entity.HasOne(d => d.Discountgroup).WithMany(p => p.Ticketdiscounts)
                .HasForeignKey(d => d.Discountgroupid)
                .HasConstraintName("ticketdiscount_discountgroupid_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

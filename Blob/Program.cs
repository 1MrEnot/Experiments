// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Blob;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

const string pgString = "Server=localhost;Port=5432;Database=blob_test;User ID=postgres;Pwd=admin;";
const int pointCount = 20_000_000;

using var sessionfactory = GetSessionfactory(pgString);
var newGraphic = AddNewGraphic(sessionfactory);
var gotGraphic = ReadGraphic(sessionfactory, newGraphic.Id);


Graphic<double> CreateGraphic()
{
    var points = new SortedSet<Point<double>>(GetPoints(pointCount));
    var graphic = new Graphic<double>
    {
        Name = "foo",
        Points = points
    };

    return graphic;
}

Graphic<double> AddNewGraphic(ISessionFactory sf)
{
    using var s = sf.OpenSession();
    using var tr = s.BeginTransaction();
    
    var sw = Stopwatch.StartNew();
    var graphic = CreateGraphic();

    Console.WriteLine($"Entitiy initialised in {sw.Elapsed}");
    
    sw.Restart();
    s.SaveOrUpdate(graphic);
    tr.Commit();
    Console.WriteLine($"Commited in {sw.Elapsed}");

    return graphic;
}

Graphic<double> ReadGraphic(ISessionFactory sf, int id)
{
    using var readSession = sf.OpenSession();
    var sw = Stopwatch.StartNew();
    var grapics = readSession.Get<Graphic<double>>(id);
    Console.WriteLine($"Get in {sw.Elapsed}");
    return grapics;
}

ISessionFactory GetSessionfactory(string connString)
{
    var sessionFactory = Fluently.Configure()
        .Database(PostgreSQLConfiguration.Standard.ConnectionString(connString))
        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<GraphicMap>())
        .BuildSessionFactory();
    
    return sessionFactory;
}

IEnumerable<Point<double>> GetPoints(int count)
{
    return Enumerable
        .Range(0, count)
        .Select(i => new Point<double>
        {
            X = i,
            Y = i
        });
}
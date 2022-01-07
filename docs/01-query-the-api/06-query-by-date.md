# Query by Date with .NET

## Query by exact date

Here we show how to query documents for a Date field equal to a specific date.

```cs
var response = await api.query(
    Predicates.At("document.type", "blog-post"),
    Predicates.At("my.blog-post.date",  new DateTime(2013, 8, 17, 0, 0))
).Submit();
```

## Other time-based predicates

Here are the other available Date query predicates.

```cs
Predicate dateBefore = Predicates.DateBefore("my.product.releaseDate", new DateTime(2014, 6, 1, 0, 0));
Predicate dateAfter = Predicates.DateAfter("my.product.releaseDate", new DateTime(2014, 1, 1, 0, 0));
Predicate dateBetween = Predicates.DateBetween("my.product.releaseDate", new DateTime(2014, 1, 1, 0, 0), new DateTime(2014, 6, 1, 0, 0));
Predicate dayOfMonth = Predicates.DayOfMonth("my.product.releaseDate", 14);
Predicate dayOfMonthAfter = Predicates.DayOfMonthAfter("my.product.releaseDate", 14);
Predicate dayOfMonthBefore = Predicates.DayOfMonthBefore("my.product.releaseDate", 14);
Predicate dayOfWeek = Predicates.DayOfWeek("my.product.releaseDate", Predicates.DayOfWeek.TUESDAY);
Predicate dayOfWeekAfter = Predicates.DayOfWeekAfter("my.product.releaseDate", Predicates.DayOfWeek.WEDNESDAY);
Predicate dayOfWeekBefore = Predicates.DayOfWeekBefore("my.product.releaseDate", Predicates.DayOfWeek.WEDNESDAY);
Predicate month = Predicates.Month("my.product.releaseDate", Predicates.Month.JUNE);
Predicate monthBefore = Predicates.MonthBefore("my.product.releaseDate", Predicates.Month.JUNE);
Predicate monthAfter = Predicates.MonthAfter("my.product.releaseDate", Predicates.Month.JUNE);
Predicate year = Predicates.Year("my.product.releaseDate", 2014);
Predicate hour = Predicates.Hour("my.product.releaseDate", 12);
Predicate hourBefore = Predicates.HourBefore("my.product.releaseDate", 12);
Predicate hourAfter = Predicates.HourAfter("my.product.releaseDate", 12);
```

Time-related predicates are strict inequalities, in other words the date (or time) corresponding to the bounds is not returned in the results. If you need an inclusive query, you should offset the bound by one day (or second).

## Orderings

Date and timestamp fields can be used to order results.

```cs
var response = await api
    .Query(Predicates.At("document.type", "blog-post"))
    .Orderings("my.blog-post.date")
    .Submit();
```

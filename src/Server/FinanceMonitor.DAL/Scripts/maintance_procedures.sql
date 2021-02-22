drop procedure if exists ProcessDailyDataIntoHistory
go
create procedure ProcessDailyDataIntoHistory @Time date
as
begin
    insert into PriceHistory (StockSymbol, Volume, Opened, Closed, High, Low, DateTime)
    select StockSymbol,
           Max(Volume)        as Volume,
           Max(Opened)        as Opened,
           Max(Closed)        as Closed,
           MAX(Price)         as High,
           Min(Price)         as Low,
           cast(Time as DATE) as DateTime

    from (select PD.StockSymbol,
                 PD.Price,
                 PD.Time,
                 First_Value(Price) over (partition by StockSymbol, cast(Time as DAte) order by Time)       as Opened,
                 First_Value(Price) over (partition by StockSymbol, cast(Time as DAte) order by Time desc)  as Closed,
                 First_Value(Volume) over (partition by StockSymbol, cast(Time as DAte) order by Time desc) as Volume

          from PriceDaily as PD

          where cast(Time as DAte) > (select Max(DateTime)
                                      from PriceHistory as PH
                                      where PH.StockSymbol = PD.StockSymbol)
            and cast(Time as Date) <= @Time
         ) as PriceDaily

    group by StockSymbol, cast(Time as DATE)
    order by cast(Time as DATE)
end

go


drop procedure if exists CalculateHistoryGraphic
go
create procedure CalculateHistoryGraphic
as
begin

    begin transaction
        truncate table SampledHistoryDataYearly;

        insert into SampledHistoryDataYearly(StockSymbol, DateTime, Volume, Opened, Closed, High, Low)
        select Symbol, DateTime, Volume, Opened, Closed, High, Low
        from Stock as S
                 cross apply GetStockHistoryDailyFunction(S.Symbol, default, default)
    commit tran
end
go
drop procedure if exists AddUser
go
create procedure AddUser @Id nvarchar(512)
as
begin
    if not exists(select Id
                  from UserProfile
                  where Id = @Id)
        begin
            insert into UserProfile
            values (@Id)
        end
end


go
drop procedure if exists GetStockDaily
go
create procedure GetStockDaily @Symbol nvarchar(512),
                               @Start datetime2,
                               @End datetime2
as
begin
    select PD.*
    from PriceDaily as PD
    where PD.StockSymbol = @Symbol
      and PD.Time between @Start and @End
    order by Time
end

go
drop procedure if exists GetUserStocks
go
create procedure GetUserStocks @UserId nvarchar(512)
as
begin
    with LastPrices
             as (
            select *,
                   first_value(Price) OVER (partition by StockSymbol order by Time desc) as LastPrice,
                   Row_number() over (partition by StockSymbol order by Time desc)       as rnumb
            from PriceDaily
        ),
         LastPrice
             as (
             select *
             from LastPrices
             where rnumb = 1
         )

    select s.Symbol,
           s.ShortName,
           s.LongName,
           SUM(UP.Count)                             as Shares,
           Sum(UP.Price * UP.Count)                  as Total,
           Sum(UP.Count * (PD.LastPrice - UP.Price)) as TotalProfit
    from UserPrice as UP
             left join LastPrice as PD on UP.StockSymbol = PD.StockSymbol
             join Stock as s on s.Symbol = UP.StockSymbol
    where UP.UserId = @UserId
    group by s.Symbol, s.ShortName, s.LongName
end


go
drop procedure if exists GetUserStockPrices
go
create procedure GetUserStockPrices @UserId nvarchar(512),
                                    @Symbol nvarchar(512)
as
begin
    select UP.*
    from UserPrice as UP
    where UserId = @UserId
      and StockSymbol = @Symbol
end

go
drop procedure if exists GetStock
go
create procedure GetStock @Symbol nvarchar(512)
as
begin
    select *
    from Stock
    where Symbol = @Symbol
end

go
drop procedure if exists AddStock
go
create procedure AddStock @Symbol nvarchar(512),
                          @Market nvarchar(512),
                          @Timezone nvarchar(512),
                          @Time datetime2,
                          @ShortName nvarchar(512),
                          @LongName nvarchar(512),
                          @Currency nvarchar(512),
                          @FinancialCurrency nvarchar(512),
                          @Language nvarchar(512),
                          @QuoteType nvarchar(512)
as
begin
    Insert into Stock (Symbol, Market, Timezone, Time, ShortName, LongName, Currency, FinancialCurrency,
                       Language, QuoteType)
    Output Inserted.*
    values (@Symbol, @Market, @Timezone, @Time, @ShortName, @LongName, @Currency, @FinancialCurrency,
            @Language, @QuoteType)
end

go
drop procedure if exists AddUserPrice
go
create procedure AddUserPrice @UserId nvarchar(512),
                              @StockSymbol nvarchar(512),
                              @Price money,
                              @Count int,
                              @DateTime datetime2
as
begin
    Insert into UserPrice(UserId, StockSymbol, Price, Count, DateTime)
    OUTPUT Inserted.*
    values (@UserId, @StockSymbol, @Price, @Count, @DateTime)
end

go
drop procedure if exists AddDailyPrice
go
create procedure AddDailyPrice @StockSymbol nvarchar(512),
                               @Ask float,
                               @Bid float,
                               @AskSize float,
                               @BidSize float,
                               @Time datetime2,
                               @Price float,
                               @Volume float
as
begin
    if not exists(select *
                  from PriceDaily
                  where StockSymbol = @StockSymbol
                    and Time = @Time)
        begin
            Insert into PriceDaily (StockSymbol, Ask, Bid, AskSize, BidSize, Time,
                                    Price, Volume)
            values (@StockSymbol, @Ask, @Bid, @AskSize, @BidSize, @Time, @Price, @Volume)
        end
end

go
drop procedure if exists InsertHistory
go
create procedure InsertHistory @StockSymbol nvarchar(512),
                               @Volume float,
                               @Opened float,
                               @Closed float,
                               @High float,
                               @Low float,
                               @DateTime datetime2
as
begin
    Insert into PriceHistory (StockSymbol, Volume, Opened, Closed, High, Low, DateTime)
    values (@StockSymbol, @Volume, @Opened, @Closed, @High, @Low, @DateTime)
end

go
drop procedure if exists GetSavedStocks
go
create procedure GetSavedStocks
as
begin
    
--     alter database PriceMonitor set compatibility_level = 150
--     drop index idx_PriceDaily_Symbol_Time on PriceDaily
--     create index idx_PriceDaily_Symbol_Time on PriceDaily(StockSymbol, Time desc)
--     include (Price, Volume);
    
    select S.*, PD.*
    from Stock as S

             join (select row_number() over (partition by PD.StockSymbol order by Time Desc) r,
                          PD.StockSymbol,
                          PD.Price  as                                                       CurrentPrice,
                          PD.Time   as                                                       CurrentTime,
                          PD.Volume as                                                       CurrentVolume
                   from PriceDaily as PD
    ) as PD on S.Symbol = PD.StockSymbol and PD.r = 1

--     select S.*, PD.*
--     from Stock as S
-- 
--              join (select distinct
--                           StockSymbol,
--                           First_Value(Price) over(partition by  StockSymbol order by Time desc)  as                                                       CurrentPrice,
--                           First_Value(Time) over(partition by  StockSymbol order by Time desc)   as                                                       CurrentTime,
--                           First_Value(Volume) over(partition by  StockSymbol order by Time desc) as                                                       CurrentVolume
--                    from PriceDaily
--     ) as PD on S.Symbol = PD.StockSymbol

--     With WholeTable as (
--         select distinct S.*,
--                         First_Value(Price) over(partition by  Symbol order by P.Time desc) as CurrentPrice,
--                         First_Value(P.Time) over(partition by  Symbol order by P.Time desc) as CurrentTime,
--                         First_Value(Volume) over(partition by  Symbol order by P.Time desc) as CurrentVolume
--         from Stock as S
--                  left join PriceDaily P on S.Symbol = P.StockSymbol    
--     )
--     select Symbol, Market, Timezone, Time, ShortName, LongName, Currency, FinancialCurrency, Language, LastClosed, LastOpened, QuoteType, Status, 
--            
--            Max(CurrentPrice), Max(CurrentTime), Max(CurrentVolume) from WholeTable
-- GROUP BY Symbol, Market, Timezone, Time, ShortName, LongName, Currency, FinancialCurrency, Language, LastClosed, LastOpened, QuoteType, Status
--     
    
    select S.Symbol, history.*
    from Stock as S
              join SampledHistoryDataYearly as history on S.Symbol = history.StockSymbol
end
go
drop procedure if exists GetStockHistoryYearly
go
create procedure GetStockHistoryYearly @Symbol nvarchar(512),
                                       @Start datetime2,
                                       @End datetime2
as
begin
    select *
    from (select ROW_NUMBER() over (partition by StockSymbol, Year(DateTime) order by DateTime desc) as r, PH.*
          from PriceHistory as PH
          where PH.StockSymbol = @Symbol
         ) as PH
    where PH.r = 1
      and DateTime between @End and @Start
    order by DateTime asc
end

go
drop procedure if exists GetStockHistoryMonthly
go
create procedure GetStockHistoryMonthly @Symbol nvarchar(512),
                                        @Start datetime2,
                                        @End datetime2
as
begin
    select *
    from (select ROW_NUMBER() over (partition by Year(DateTime), Month(DateTime) order by DateTime desc) as r, PH.*
          from PriceHistory as PH
          where PH.StockSymbol = @Symbol
         ) as PH
    where PH.r = 1
      and DateTime between @End and @Start
    order by DateTime asc
end

go
drop function if exists GetStockHistoryDailyFunction
go
create function GetStockHistoryDailyFunction(@Symbol nvarchar(512),
                                             @Start datetime2 = '9999-01-01',
                                             @End datetime2 = '0001-01-01')
    returns table
        as
        return
            (
                with dailyHistory as (
                    select PH.*,
                           NTILE(45) OVER (order by DateTime asc) AS Quartile
                    from PriceHistory as PH
                    where PH.StockSymbol = @Symbol
                      and DateTime between @End and @Start
                )


                select *
                from (
                         select *,
                                first_value(RowNumber) over (partition by Quartile order by RowNumber desc) as LastRow
                         from (
                                  select *, Row_Number() over (partition by Quartile order by Quartile asc) as RowNumber
                                  from dailyHistory
                              ) as t
                     ) as t1
                where (t1.RowNumber = 1 and Quartile = 1)
                   or t1.RowNumber = CEILING(t1.LastRow / 2)
                   or (t1.RowNumber = t1.LastRow)

            )
go
drop procedure if exists GetStockHistoryDaily
go
create procedure GetStockHistoryDaily @Symbol nvarchar(512),
                                      @Start datetime2 = '0001-01-01',
                                      @End datetime2 = '9999-01-01'
as
begin
    with dailyHistory as (
        select PH.*,
               NTILE(45) OVER (order by DateTime asc) AS Quartile
        from PriceHistory as PH
        where PH.StockSymbol = @Symbol
          and DateTime between @End and @Start
    )


    select *
    from (
             select *, first_value(RowNumber) over (partition by Quartile order by RowNumber desc) as LastRow
             from (
                      select *, Row_Number() over (partition by Quartile order by Quartile asc) as RowNumber
                      from dailyHistory
                  ) as t
         ) as t1
    where (t1.RowNumber = 1 and Quartile = 1)
       or t1.RowNumber = CEILING(t1.LastRow / 2)
       or (t1.RowNumber = t1.LastRow)

    order by DateTime asc
end

go
drop procedure if exists GetStockHistory
go
create procedure GetStockHistory @Symbol nvarchar(512),
                                 @Type int = 0,
                                 @Start datetime2 = null,
                                 @End datetime2 = null
as
begin
    if @Type = 0
        exec dbo.GetStockHistoryDaily @Symbol, @Start, @End
    else
        if @Type = 1
            exec dbo.GetStockHistoryMonthly @Symbol, @Start, @End
        else
            exec dbo.GetStockHistoryYearly @Symbol, @Start, @End
end
go

drop procedure if exists GetStocksWithoutHistory
go
create procedure GetStocksWithoutHistory
as
begin
    select S.*
    from Stock as S
             left join PriceHistory PH on S.Symbol = PH.StockSymbol
    where PH.StockSymbol is null
end
go
drop procedure if exists GetStocks
go
create procedure GetStocks
as
begin
    select S.Symbol from Stock as S
end
go
drop procedure if exists UpdateStockStatus
go
create procedure UpdateStockStatus @Symbol nvarchar(512),
                                   @Status nvarchar(50)
as
begin
    update Stock
    set Status = @Status
    where Symbol = @Symbol
end
go
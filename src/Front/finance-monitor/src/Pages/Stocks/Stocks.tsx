import * as React from 'react'
import {useEffect} from 'react'
import {DetailsList, ProgressIndicator, SelectionMode,IColumn} from "@fluentui/react";
import {Stock, StockClient, StockListItemDto} from "../../Api/ApiClients";
import {host} from "../../Api/Consts";
import { useHistory } from 'react-router-dom';
import {CartesianGrid, Legend, Line, LineChart, Tooltip, XAxis, YAxis} from "recharts";
import moment from "moment";
// @ts-ignore
import { usePageVisibility } from 'react-page-visibility';

interface IProps {

}

const defualtColumn = (name: string):IColumn =>{
    return {
        fieldName: name,
        name: name,
        key: name,
        minWidth: 0,
        isResizable: true,
        maxWidth: 100
    }
}

const columns :IColumn[] = [
    defualtColumn('symbol'),
    defualtColumn('longName'),
    defualtColumn('shortName'),
    defualtColumn('quoteType'),
    defualtColumn('currency'),
    defualtColumn('currentPrice'),
    defualtColumn('currentVolume'),
    {...defualtColumn('currentTime'),
    minWidth: 140},
    defualtColumn('status'),
    defualtColumn('financialCurrency'),
    defualtColumn('market'),
    defualtColumn('timezone'),
    {
        ...defualtColumn('fullHistory'),
        minWidth: 500,
        onRender: (item, index, column) => {
            return (<img key={item.symbol} src={`https://localhost:8181/api/preview/${item.symbol}`} height={100} width={500} />)
            // return (<LineChart
            //     width={500}
            //     height={100}
            //     data={item[column!.fieldName!]}
            // >
            //     <XAxis dataKey="time" domain={['dataMin', 'dataMax']}/>
            //     <YAxis type="number" domain={['dataMin', 'dataMax']} yAxisId={0}/>
            //     <CartesianGrid stroke="#f5f5f5"/>
            //     <Line type="monotone" dataKey="closed" stroke="#ff7300" dot={false} yAxisId={0}/>
            // </LineChart>)
        }
    }
]

export const Stocks = (props: IProps) => {

    const history = useHistory();

    const [isLoading, setLoading] = React.useState(true);
    const [stocks, setStocks] = React.useState<StockListItemDto[]>([])

    const isVisible = usePageVisibility()


    useEffect(() => {
        if (!isVisible){
            return ;
        }
        new StockClient({},host).list()
            .then(stocks => {
                setStocks(stocks);
            })
            .finally(() => setLoading(false))
    }, [isVisible])


    const refreshData = async ()=> {
        await new StockClient({},host).list()
            .then(stocks => {
                setStocks(stocks);
            })
    };

    useEffect(()=>{
        if (!isVisible){
            console.log("page is not visible, do not pull data")
            return;
        }

        console.log("page is visible, pull data")
        const interval = setInterval(refreshData, 30*1000)

        return ()=> clearInterval(interval)
    }, [isVisible])

    const onRowClick = (item: Stock, index?: number) => {
        if (!item)
            return;

        history.push(`${history.location.pathname}/${item.symbol}` )
    }

    if (isLoading) {
        return (<ProgressIndicator label={"Loading"}/>)
    }

    return (<><h1>Stocks ({stocks?.length ?? 0})</h1>
        <DetailsList
            items={stocks}
            selectionMode={SelectionMode.none}
            compact={true}
            onActiveItemChanged={onRowClick}
            columns={columns}
        ></DetailsList></>)
}
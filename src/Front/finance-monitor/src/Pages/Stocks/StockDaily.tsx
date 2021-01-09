import * as React from 'react'
import {useParams} from "react-router-dom";
import {useEffect, useMemo, useState} from "react";
import {PriceDaily, PriceHistory, StockClient} from "../../Api/ApiClients";
import {host} from "../../Api/Consts";
import {ProgressIndicator} from "@fluentui/react";
import {CartesianGrid, Legend, Line, LineChart, Tooltip, XAxis, YAxis} from "recharts";
import moment from "moment";

interface IProps{

}

const formatXAxis = (tickItem: Date) => {
    return moment(tickItem).local(true).format('LT');
}

export const StockDaily = (props: IProps)=>{
    const {symbol} = useParams<{ symbol: string }>()
    const [loading, setLoading] = useState(true);
    const [history, setHistory] = useState<PriceDaily[]>()


    useEffect(() => {
        new StockClient({}, host).daily(symbol)
            .then(x => {
                setHistory(x.map(h => {
                    return {
                        ...h,
                        time: moment.utc(h.time).local().toDate(),
                    }
                }));
            })
            .finally(() => setLoading(false))

    }, [])



    if (loading)
        return <ProgressIndicator label={"Loading"}/>

    return (<>

        <LineChart
            width={1000}
            height={400}
            data={history}
            margin={{top: 5, right: 20, left: 10, bottom: 5}}

        >
            <XAxis  dataKey="time" domain={['dataMin', 'dataMax']} tickFormatter={formatXAxis}/>
            <YAxis type="number" domain={['dataMin', 'dataMax']} yAxisId={0} />
            <Tooltip
                labelFormatter={(label, payload) => {
                    return moment(label).local(true).format('LTS')
                }}/>
            <CartesianGrid stroke="#f5f5f5"/>
            <Line type="monotone" dataKey="price" stroke="#ff7300" yAxisId={0} />
            {/*<Line type="monotone" dataKey="volume" stroke="#387908" yAxisId={0}/>*/}
            <Legend/>
        </LineChart>
    </>)
}
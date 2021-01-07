import * as React from 'react'
import {useEffect, useMemo, useState} from 'react'
import {useParams} from "react-router-dom";
import {ProgressIndicator} from "@fluentui/react";
import {PriceHistory, StockClient} from "../../Api/ApiClients";
import {host} from "../../Api/Consts";
import {CartesianGrid, Legend, Line, LineChart, Tooltip, XAxis, YAxis} from "recharts";
import moment from "moment";

interface IProps {

}

const formatXAxis = (tickItem: Date) => {
    return moment(tickItem).format('D MMM');
}

export const StockHistory = (props: IProps) => {

    const {symbol} = useParams<{ symbol: string }>()
    const [loading, setLoading] = useState(true);
    const [history, setHistory] = useState<PriceHistory[]>()

    useEffect(() => {
        new StockClient(host).history(symbol)
            .then(x => {
                setHistory(x.map(h => {
                    return {
                        ...h,
                        dateTime: new Date(h.dateTime)
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
            <XAxis  dataKey="dateTime" domain={['dataMin', 'dataMax']} tickFormatter={formatXAxis}/>
            <YAxis type="number" domain={['dataMin', 'dataMax']} yAxisId={0} />
            <Tooltip
            labelFormatter={(label, payload) => {
                return moment(label).local(false).format("L")
            }}/>
            <CartesianGrid stroke="#f5f5f5"/>
            <Line type="monotone" dataKey="high" stroke="#ff7300" yAxisId={0} />
            <Line type="monotone" dataKey="low" stroke="#387908" yAxisId={0}/>
            <Legend/>
        </LineChart>
    </>)
}
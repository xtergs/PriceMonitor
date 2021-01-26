import * as React from 'react'
import {useEffect, useState} from 'react'
import {useParams} from "react-router-dom";
import {PriceDaily, StockClient} from "../../Api/ApiClients";
import {host} from "../../Api/Consts";
import {DefaultButton, ProgressIndicator, Stack, Text} from "@fluentui/react";
import {CartesianGrid, Legend, Line, LineChart, Tooltip, XAxis, YAxis} from "recharts";
import moment from "moment";

interface IProps {

}

const formatXAxis = (tickItem: Date) => {
    return moment(tickItem).local(true).format('LT');
}

export const StockDaily = (props: IProps) => {
    const {symbol} = useParams<{ symbol: string }>()
    const [loading, setLoading] = useState(true);
    const [history, setHistory] = useState<PriceDaily[]>()
    const [day, setDay] = useState(moment().utc().toDate())


    useEffect(() => {
        new StockClient({}, host).daily(symbol, day)
            .then(x => {
                setHistory(x.map(h => {
                    return {
                        ...h,
                        time: moment.utc(h.time).local().toDate(),
                    }
                }));
            })
            .finally(() => setLoading(false))

    }, [day])

    const changeDay = (diff: number) => {
        setDay(moment(day).add("days", diff).toDate())
    }


    if (loading)
        return <ProgressIndicator label={"Loading"}/>

    return (<>
        <Text>{moment(day).local(false).format("LL")}</Text>
        <Stack>
            <DefaultButton onClick={() => changeDay(-1)}>Prev</DefaultButton>
            <DefaultButton onClick={() => changeDay(1)}>Next</DefaultButton>
        </Stack>
        <LineChart
            width={1000}
            height={400}
            data={history}
            margin={{top: 5, right: 20, left: 10, bottom: 5}}

        >
            <XAxis dataKey="time" domain={['dataMin', 'dataMax']} tickFormatter={formatXAxis}/>
            <YAxis type="number" domain={['dataMin', 'dataMax']} yAxisId={0}/>
            <Tooltip
                labelFormatter={(label, payload) => {
                    return moment(label).local(true).format('LTS')
                }}/>
            <CartesianGrid stroke="#f5f5f5"/>
            <Line type="monotone" dataKey="price" stroke="#ff7300" yAxisId={0}/>
            {/*<Line type="monotone" dataKey="volume" stroke="#387908" yAxisId={0}/>*/}
            <Legend/>
        </LineChart>
    </>)
}
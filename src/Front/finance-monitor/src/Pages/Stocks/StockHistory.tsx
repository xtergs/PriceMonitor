import * as React from 'react'
import {useEffect, useState} from 'react'
import {useParams} from "react-router-dom";
import {Dropdown, ProgressIndicator, Spinner, Stack} from "@fluentui/react";
import {HistoryType, PriceHistory, StockClient} from "../../Api/ApiClients";
import {host} from "../../Api/Consts";
import {CartesianGrid, Legend, Line, LineChart, ReferenceArea, Tooltip, XAxis, YAxis} from "recharts";
import moment from "moment";

interface IProps {

}

const dropDownOptions = [{
    key: HistoryType.Day,
    text: HistoryType.Day
},
    {
        key: HistoryType.Month,
        text: HistoryType.Month
    },
    {
        key: HistoryType.Year,
        text: HistoryType.Year
    }];
export const StockHistory = (props: IProps) => {

    const {symbol} = useParams<{ symbol: string }>()
    const [loading, setLoading] = useState(true);
    const [refreshing, setRefreshing] = useState(false);
    const [history, setHistory] = useState<PriceHistory[]>()
    const [startDate, setStart] = useState(new Date())
    const [endDate, setEnd] = useState(moment.utc().add("days", -90).toDate())
    const [type, setType] = useState(HistoryType.Day)
    const [leftArea, setLeftArea] = useState<string | null>(null);
    const [rightArea, setRightArea] = useState<string | null>(null)


    const formatXAxis = (tickItem: Date) => {
        if (type === HistoryType.Year) {
            return moment(tickItem).format('YYYY');
        }
        if (type === HistoryType.Month) {
            return moment(tickItem).format('MMM');
        }
        return moment(tickItem).format('D MMM');
    }

    useEffect(() => {
        setRefreshing(true)
        new StockClient({}, host).history(symbol, type, startDate, endDate)
            .then(x => {
                setHistory(x.map(h => {
                    return {
                        ...h,
                        dateTime: moment.utc(h.dateTime).local().toDate()
                    }
                }));
            })
            .finally(() => {

                setLoading(false)
                setRefreshing(false)
            })

    }, [startDate, endDate, type])

    const handleMapClick = (data: any) => {

    }

    const zoom = () => {
        if (leftArea === rightArea || !rightArea || !leftArea) {
            setLeftArea(null);
            setRightArea(null);
            return;
        }

        let endArea = moment(leftArea);
        let startArea = moment(rightArea)
        if (endArea! > startArea!) [endArea, startArea] = [startArea, endArea];

        setStart(startArea.toDate());
        setEnd(endArea.toDate())
        setLeftArea(null);
        setRightArea(null)
    }

    if (loading)
        return <ProgressIndicator label={"Loading"}/>
    return (<>
        <Stack>

            <span>{`${moment(startDate).local().format("LL")} - ${moment(endDate).local().format("LL")}`}</span>
            <Dropdown options={dropDownOptions} selectedKey={type}
                      onChanged={x => {
                          const t = x.key as HistoryType;
                          setType(t)
                          if (t !== HistoryType.Day){
                              setEnd(moment.utc().add("years",-25).toDate());
                              setStart(moment.utc().toDate());

                          }
                      }}/>
            {refreshing ? (<Spinner/>) : (

                <div style={{userSelect: 'none'}}>
                    <LineChart
                        width={1000}
                        height={400}
                        data={history}
                        margin={{top: 5, right: 20, left: 10, bottom: 5}}
                        // onClick={(e: any, d: any) => {
                        //     console.log(e);
                        //     console.log(d)
                        // }}
                        onMouseDown={(e: any) => {
                            if (!e) {
                                return;
                            }
                            setLeftArea(e.activeLabel)
                        }}
                        onMouseMove={(e: any) => {
                            if (!e)
                                return;
                            leftArea && setRightArea(e.activeLabel)
                        }}
                        onMouseUp={zoom}
                    >
                        <XAxis dataKey="dateTime" domain={['dataMin', 'dataMax']} tickFormatter={formatXAxis}/>
                        <YAxis type="number" domain={['dataMin', 'dataMax']} yAxisId={0}/>
                        <Tooltip
                            labelFormatter={(label, payload) => {
                                return moment(label).local(false).format("L")
                            }}/>
                        <CartesianGrid stroke="#f5f5f5"/>
                        <Line type="monotone" dataKey="high" stroke="#ff7300" yAxisId={0}/>
                        <Line type="monotone" dataKey="low" stroke="#387908" yAxisId={0}/>
                        {
                            (!!leftArea && !!rightArea) ? (
                                <ReferenceArea yAxisId="0" x1={new Date(leftArea).toString()}
                                               x2={new Date(rightArea).toString()} stroke="#8884d8"
                                               strokeOpacity={0.3}/>) : null

                        }
                        <Legend/>
                    </LineChart>
                </div>)}
        </Stack>
    </>)
}
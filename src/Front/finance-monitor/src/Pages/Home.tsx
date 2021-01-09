import * as React from 'react'
import {Stack} from "@fluentui/react";
import {Authorize} from "./Authorize/Authorize";

export const Home = () => {


    return (
        <Stack
            horizontalAlign="center"
            verticalAlign="center"
            verticalFill
            styles={{
                root: {
                    width: '960px',
                    margin: '0 auto',
                    textAlign: 'center',
                    color: '#605e5c'
                }
            }}
            gap={15}
        >
            <Authorize/>
            <img
                src="https://raw.githubusercontent.com/Microsoft/just/master/packages/just-stack-uifabric/template/src/components/fabric.png"
                alt="logo"
            />
        </Stack>
    )
}
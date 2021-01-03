import * as React from 'react'
import {useContext, useEffect} from 'react'
import {PrimaryButton, Separator, Stack, Text} from "@fluentui/react";
import {Register} from "../Register/Register";
import {AppContext, GetManager} from "../../App";

interface IProps {

}

export const Authorize = (props: IProps) => {

    const context = useContext(AppContext)

    const login = async () => {

        const manager = GetManager()

        await manager.signinRedirect()
    }

    const logout = async () => {
        const manager = GetManager();
        await manager.signoutRedirect();
    }

    return (<>
        <Stack>
            <Text>Welcome to PriceMonitor app</Text>
            {!!context.user ? (
                <>
                    <Text>You are authorized</Text>
                    <PrimaryButton onClick={logout}>Log Out</PrimaryButton>
                </>
            ) : (
                <>
                    <Text>Register</Text>
                    <Register/>
                    <Separator title={"OR"}>OR</Separator>
                    <PrimaryButton onClick={login}>Sign In</PrimaryButton>
                </>
            )}
        </Stack>
    </>)
}
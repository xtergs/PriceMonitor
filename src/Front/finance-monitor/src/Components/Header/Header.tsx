import * as React from "react";
import {useContext} from "react";
import {DefaultButton, Stack, Text} from '@fluentui/react'
import {AppContext, GetManager} from "../../App";
import {Link} from "react-router-dom";

export const Header = () => {

    const context = useContext(AppContext)

    const isAuthorized = !!context.user;

    const logout = async () => {
        const manager = GetManager();
        await manager.signoutRedirect();
    }

    return (<>
        <Stack horizontal gap={15}>
            <Link to={"/"}>Home</Link>
            {isAuthorized ? <Link to={"/profile"}>Profile</Link> : null}
            <Link to={"/stocks"}>Stocks</Link>
            <Text>Header!</Text>
            {isAuthorized ? <DefaultButton onClick={logout}>Log Out</DefaultButton> : null}
        </Stack>
    </>)
}
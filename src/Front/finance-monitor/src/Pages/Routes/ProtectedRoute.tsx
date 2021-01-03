import * as React from "react";
import {useContext} from "react";
import {AppContext} from "../../App";
import {Redirect, Route} from "react-router-dom";
import {RouteProps} from "react-router";


export const ProtectedRoute = (props: RouteProps) => {
    const context = useContext(AppContext)

    if (context.user) {
        return <Route {...props}/>
    }
    return <Redirect to={{
        pathname: "/",
        state: {
            from: props.location
        }
    }}/>
}
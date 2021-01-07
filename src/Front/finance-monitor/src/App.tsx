import React, {useEffect} from 'react';
import {FontWeights} from '@fluentui/react';
import {User, UserManager} from "oidc-client";
import {BrowserRouter as Router, Route, Switch} from "react-router-dom";
import {Home} from "./Pages/Home";
import {SigninResponse} from "./Pages/Callbacks/SigninResponse";
import {Logout} from "./Pages/Callbacks/Logout";
import {Header} from "./Components/Header/Header";
import {Profile} from "./Pages/Profile/Profile";
import {Stocks} from "./Pages/Stocks/Stocks";
import {ProtectedRoute} from "./Pages/Routes/ProtectedRoute";
import {StockDetails} from "./Pages/Stocks/StockDetails";
import {StockHistory} from "./Pages/Stocks/StockHistory";
import {StockDaily} from "./Pages/Stocks/StockDaily";

const boldStyle = {root: {fontWeight: FontWeights.semibold}};

const manager = new UserManager({
    client_id: "interactive",
    client_secret: "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0",
    scope: "openid profile scope2",
    authority: "https://localhost:5002",
    redirect_uri: "http://localhost:3000/signin",
    response_type: "code",
    post_logout_redirect_uri: "http://localhost:3000/logout-callback"

});

export const GetManager = () => manager;

export interface IContext {
    user: User | null,
    updateUser: (user: User | null) => void
}


export const AppContext = React.createContext<IContext>({
    user: null,
    updateUser: () => {
    }
})

export const App: React.FunctionComponent = () => {


    const [context, setContext] = React.useState<IContext>({
        user: null,
        updateUser: user => {
            if (user && user.expired)
                user = null
            setContext({...context, user: user})
        }
    });

    useEffect(() => {
        GetManager().getUser().then(u => {
            context.updateUser(u);
        })
    }, [])

    return (
        <AppContext.Provider value={context}>
            <Router>
                <Header/>
                <Switch>
                    <Route exact={true} path={"/signin"}>
                        <SigninResponse/>
                    </Route>
                    <Route exact={true} path={"/logout"}>
                        <Logout/>
                    </Route>
                    <ProtectedRoute exact={true} path={"/profile"}>
                        <Profile/>
                    </ProtectedRoute>
                    <Route exact={true} path={"/stocks"}>
                        <Stocks/>
                    </Route>
                    <Route exact={true} path={"/stocks/:symbol"}>
                        <StockDetails />
                    </Route>
                    <Route exact={true} path={"/stocks/:symbol/history"}>
                        <StockHistory />
                    </Route>
                    <Route exact={true} path={"/stocks/:symbol/daily"}>
                        <StockDaily />
                    </Route>
                    <Route path={"/"}>
                        <Home/>
                    </Route>
                </Switch>
            </Router>
        </AppContext.Provider>
    );
};

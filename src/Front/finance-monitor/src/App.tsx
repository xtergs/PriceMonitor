import React from 'react';
import {Stack, Text, FontWeights, DefaultButton} from 'office-ui-fabric-react';
import {AddStockForm} from "./Components/AddStockForm/AddStockForm";
import {UserManager} from "oidc-client";
import {
  BrowserRouter as Router,
  Switch,
  Route,
  Link
} from "react-router-dom";
import {Home} from "./Pages/Home";
import {SigninResponse} from "./Pages/Login/SigninResponse";

const boldStyle = { root: { fontWeight: FontWeights.semibold } };

const manager = new UserManager({
    client_id: "interactive",
    client_secret: "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0",
    scope: "openid profile scope2",
    authority: "https://localhost:5002",
    redirect_uri: "http://localhost:3000/signin",
    response_type: "code",

});

export const GetManager = ()=> manager;

export const App: React.FunctionComponent = () => {




  return (
      <Router>
        <Switch>
          <Route path={"/signin"} >
            <SigninResponse />
          </Route>
          <Route path={"/"} >
            <Home />
          </Route>
        </Switch>
      </Router>

  );
};

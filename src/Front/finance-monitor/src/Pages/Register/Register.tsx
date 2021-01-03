import * as React from 'react'
import {PrimaryButton, Stack, TextField, ProgressIndicator} from '@fluentui/react';
import {Api} from "../../Api/Api";

interface IProps {

}

export const Register = (props: IProps) => {

    const [isSubmitting, setSubmitting] = React.useState(false);
    const [email, setEmail] = React.useState('');
    const [password, setPassword] = React.useState('');

    const handleRegisterClick = async () => {
        if (isSubmitting)
            return;

        setSubmitting(true)
        try {
            await Api.register(email, password);
            setEmail('');
            setPassword('')
            alert("Now you can Sign In")
        }
        finally {
            setSubmitting(false)
        }
    }

    return (<>
        <Stack>
            <TextField label={"Email"} type={"email"} value={email}
                       onChange={(e, v) => setEmail(v || '')}/>
            <TextField label={"Password"} type={"password"} value={password}
                       onChange={(e, v) => setPassword(v || '')}/>
            <PrimaryButton onClick={handleRegisterClick} disabled={isSubmitting}>Register</PrimaryButton>
            {isSubmitting ?? <ProgressIndicator  />}
        </Stack>
    </>)
}
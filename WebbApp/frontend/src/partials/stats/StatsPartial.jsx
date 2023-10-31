import { useEffect,  useContext, useState } from "react";

const StatsPartial = ({ jwt }) =>{
    const [errorMessage, setErrorMessage] = useState(null);
    const [stats, setStats] = useState(null);

    
    useEffect(() =>{
        SetStatsAsync(jwt, setErrorMessage, setStats);
    },[]);


    return(
        <div className="stats-partial">
            <h1>Game stats</h1>
            {
                stats === null ?
                <>
                    
                </>
                :
                <>
                    <p>Wins: {stats.wins}</p>
                    <p>Losses: {stats.losses}</p>
                </>
            }
            {
                errorMessage === null ? <></> : <><p>{errorMessage}</p></>
            }
        </div>
    );
}

//Gets user stats if user is premium
const SetStatsAsync = async (jwt, setErrorMessage, setStats) =>{
    try
    {
        let response;
        let result; 

        //Get rsa public key from server
        response = await fetch("https://localhost:5000/api/user/stats", {
            headers: {
                Authorization: "Bearer " + jwt
            }
        });
        if(response.ok !== true)
        {
            //If response is 403 then the user does not have premium role
            if(response.status === 403)
            {
                setErrorMessage("Please buy premium to access your game stats.");
                return;
            }

            setErrorMessage("Something went wrong while trying to load your game stats.");    
            return;
        }

        result = await response.json();

        setStats(result);
    }
    catch(error)
    {
        setErrorMessage("Something went wrong while trying to load your game stats.");
    }
}

export default StatsPartial;
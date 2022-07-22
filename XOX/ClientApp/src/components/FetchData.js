import React, { Component } from 'react';
import { Container } from './Container';

export class FetchData extends Component {
    static displayName = FetchData.name;

    constructor(props) {
        super(props);
        this.state = { data: Object, currentPlayer: Object, error: '' };
    }

    componentDidMount() {
        this.getUser();
        this.updatePlayground();
    }

    async connect() {
        let sessionId = document.getElementById('sessionId').value;
        const response = await fetch('session/connect?sessionId=' + sessionId, {
            method: 'POST',
        });
        this.proceedDataResponse(response);
    }

    async retreat() {
        const response = await fetch('session/retreat?sessionId=' + this.state.data.Id, {
            method: 'POST',
        });
        this.proceedDataResponse(response);
    }

    async start() {
        const response = await fetch('session/start', {
            method: 'POST',
        });
        this.proceedDataResponse(response);
    }

    async setMark(x, y) {
        const response = await fetch('session/setMark?sessionId=' + this.state.data.Id + '&x=' + x + '&y=' + y, {
            method: 'POST',
        });
        this.proceedDataResponse(response);
    }

    renderPlayground() {
        let data = this.state.data;
        let content =
            <div className='data-content'>
                {this.renderPlayers(data)}
                <div className='info-gray'>Session №: {this.state.data.Id}</div>
                {this.renderSessionState()}
                <table className='playground' aria-labelledby='tableLabel'>
                    <tbody>
                        {data.Field.Cells.map((row) =>
                            <tr key={row[0].x}>{row.map((cell) =>
                                <td key={cell.x + '' + cell.y} onClick={() => this.setMark(cell.x, cell.y)}> {cell.Value}</td>)}
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        return content;
    }

    renderNoSessionPlayer() {
        let content =
            <div id='players'>
                {this.renderPlayer(this.state.currentPlayer, false)}
            </div>
        return content;
    }

    renderPlayers(data) {
        let playerCurrent = '';
        let playerAnother = '';
        if (data.Player1 != null && this.state.currentPlayer.Id === data.Player1.Id) {
            playerCurrent = data.Player1;
            playerAnother = data.Player2;
        }
        else if (data.Player2 != null && this.state.currentPlayer.Id === data.Player2.Id) {
            playerCurrent = data.Player2;
            playerAnother = data.Player1;
        }
        let content =
            <section aria-labelledby='playersLabel'>
                {
                    (playerCurrent == null) ? (
                        <div id='players'>
                            <div id='Player1' className='column'>{this.renderPlayer(data.Player1)}</div>
                            <div id='Player2' className='column'>{this.renderPlayer(data.Player2)}</div>
                        </div>
                    )
                        : (
                            <div id='players'>
                                <div id='PlayerCurrent' className='column'>{this.renderPlayer(playerCurrent)}</div>
                                <div id='Player2' className='column'>{this.renderPlayer(playerAnother)}</div>
                            </div>
                        )
                }
            </section>
        return content;
    }

    renderPlayer(playerData, isStarted = true) {
        if (playerData == null)
            return <div></div>;
        let content =
            <div id='player'>
                <div>
                    <div id='Name'>{playerData.Name}</div>
                    <div id='Mark'>{playerData.Mark}</div>
                </div>
                {(!isStarted) ?
                    <Container user={this.state.currentPlayer} onSubmit={(event) => this.change(event)} /> : null}
            </div>
        return content;
    }

    updatePlayground() {
        var source = new EventSource('/session-sse');

        source.onmessage = function (event) {
            this.setState({ data: JSON.parse(event.data) });
        }.bind(this);
    }

    render() {
        let contents =
            <div className='container'>
                <div id='sessionControl'>
                    <input type='number' id='sessionId' className='control' placeholder='Enter session no. to connect to'></input>
                    <button id='connect' className='control btn' onClick={() => this.connect()}>Connect</button>
                    <button id='start' className='control btn' onClick={() => this.start()}>Start new</button>
                    {(((this.state.data == null) || (this.state.data.Id == null)) || this.state.data.State !== 2) ?
                        null :
                        <button id='retreat' className='control btn secondary-btn' onClick={() => this.retreat()}>Retreat</button>}
                </div>
                {(this.state.error) ?
                    <div id='error' key='error' className='error'>
                        <div>
                            {this.state.error}
                        </div>
                        <button className='nocolor-btn' onClick={() => this.closeError()} >
                            X
                        </button>
                    </div> : null}
                {((this.state.data == null) || (this.state.data.Id == null)) ? (this.renderNoSessionPlayer()) : (this.renderPlayground())}
            </div>;

        return (contents);
    }

    async loadSession() {
        const response = await fetch('session/get?sessionId=' + this.state.data.Id);
        await this.proceedDataResponse(response);
    }

    async getUser() {
        const response = await fetch('user/getOrCreate');
        this.setState({ currentPlayer: await response.json(), error: null });
    }

    getWinnerName() {
        if ((this.state.data == null) || (this.state.data.Id == null)) return '';
        if (this.state.data.IsActivePlayer1 === true)
            return this.state.data.Player1.Name;
        //if winner isn't the 1st player then the 2nd
        return this.state.data.Player2.Name;
    }

    renderSessionState() {
        if ((this.state.data == null) || (this.state.data.Id == null)) return null;
        switch (this.state.data.State) {
            case 3:
                return <div className='info-green'>Game is over. Winner: {this.getWinnerName()} </div>
            case 4:
                return <div className='info-green'>Game ended in a draw</div>
            default:
                return null;
        }
    }

    async change(event) {
        event.preventDefault(event);
        const response = await fetch('user/change?name=' + event.target.name.value + '&mark=' + event.target.mark.value, {
            method: 'POST',
        });
        await this.proceedUserDataResponse(response);
    }

    async proceedDataResponse(response) {
        if (response.status >= 500) {
            this.setState({ data: this.state.data, error: "Internal server error: " + response.status });
        }
        else if (response.status >= 400) {
            this.setState({ data: this.state.data, error: await response.text() });
        }
        else {
            this.setState({ data: await response.json(), error: null });
        }
    }

    async proceedUserDataResponse(response) {
        if (response.status >= 500) {
            this.setState({ currentPlayer: this.state.currentPlayer, error: "Internal server error: " + response.status });
        }
        else if (response.status >= 400) {
            this.setState({ currentPlayer: this.state.currentPlayer, error: await response.text() });
        }
        else {
            this.setState({ currentPlayer: await response.json(), error: null });
        }
    }

    closeError(){
        this.setState({error: null });
    }
}
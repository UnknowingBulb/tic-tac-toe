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

    async connect(sessionId) {
        const response = await fetch('connect?sessionId=' + sessionId, {
            method: 'POST',
        });
        this.proceedDataResponse(response);
    }

    async start() {
        const response = await fetch('start', {
            method: 'POST',
        });
        this.proceedDataResponse(response);
    }

    async setMark(x, y) {
        const response = await fetch('setMark?sessionId=' + this.state.data.Id + '&x=' + x + '&y=' + y, {
            method: 'POST',
        });
        this.proceedDataResponse(response);
    }

    renderPlayground(data) {
        let content =
            <div>
                {this.renderPlayers(data)}
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
            <div>
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
            <section className='row' aria-labelledby='playersLabel'>
                {
                    (playerCurrent == null) ? (
                        <div>
                            <div id='Player1' className='column'>{this.renderPlayer(data.Player1)}</div>
                            <div id='Player2' className='column'>{this.renderPlayer(data.Player2)}</div>
                        </div>
                    )
                        : (
                            <div>
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
            <div>
                <div id='Name'>{playerData.Name}</div>
                <div><div id='Mark'>{playerData.Mark}</div>{(!isStarted) ?
                    <Container user={this.state.currentPlayer} onSubmit={(event) => this.change(event)} /> : <div></div>}
                </div>
            </div>
        return content;
    }

    updatePlayground() {
        var source = new EventSource('/session');

        source.onmessage = function (event) {
            this.setState({ data: JSON.parse(event.data) });
        }.bind(this);
    }

    render() {
        let contents =
            <div>
                <input type='number' id='sessionId'></input>
                <button id='connect' onClick={() => this.connect(document.getElementById('sessionId').value)}>Connect</button>
                <button id='start' onClick={() => this.start()}>Start</button>
                <div id='error' key='error' className='error'>{this.state.error}</div>
                {((this.state.data == null) || (this.state.data.Id == null)) ? (this.renderNoSessionPlayer()) : (this.renderPlayground(this.state.data))}
            </div>;

        return (contents);
    }

    async loadSession() {
        const response = await fetch('getSession?sessionId=' + this.state.data.Id);
        await this.proceedDataResponse(response);
    }

    async getUser() {
        const response = await fetch('getOrCreate');
        this.setState({ currentPlayer: await response.json(), error: null });
    }

    async change(event) {
        event.preventDefault(event);
        const response = await fetch('change?name=' + event.target.name.value + '&mark=' + event.target.mark.value, {
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
}

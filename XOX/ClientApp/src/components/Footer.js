import React, { Component } from 'react';

export class Footer extends Component {

  render () {
    return (
        <footer id='bottom'>
            <div>Site uses cookies. Leave if you don't agree with this.</div>
            {(window.location.href.includes('localhost')) === true ? <div>github repo <a href="https://github.com/UnknowingBulb/tic-tac-toe">tic-tac-toe</a> </div> : null }
        </footer>
    );
  }
}
